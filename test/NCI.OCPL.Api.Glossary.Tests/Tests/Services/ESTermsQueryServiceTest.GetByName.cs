using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json.Linq;
using Xunit;

using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;
using NCI.OCPL.Api.Common;
using System.Text;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public partial class ESTermsQueryServiceTest
    {

        public static IEnumerable<object[]> GetByNameData => new[] {
            new object[] { new GetByName_s_1() },
            new object[] { new GetByName_s_phase_fraction() }
        };

        /// <summary>
        /// Test failure to connect to Elasticsearch for GetByName.
        /// </summary>
        [Theory]
        [InlineData(401)]
        [InlineData(403)]
        [InlineData(500)]
        [InlineData(502)]
        [InlineData(503)]
        public async void GetByName_TestAPIConnectionFailure(int returnStatus)
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("An error message"),
                statusCode: returnStatus,
                exception: null,
                contentType: "text/plain"
            );

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            await Assert.ThrowsAsync<APIInternalException>(
                () => termsClient.GetByName("cancer.gov", AudienceType.Patient, "en", "s-1")
            );
        }

        /// <summary>
        /// Test receiving an invalid response from ES in GetByName.
        /// </summary>
        [Fact]
        public async void GetByName_TestInvalidResponse()
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("Not the server you were looking for"),
                statusCode: 200,
                exception: null,
                contentType: "text/plain"
            );

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIInternalException ex = await Assert.ThrowsAsync<APIInternalException>(
                () => termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1")
            );
        }

        /// <summary>
        /// Test that GetByName Request for Elasticsearch is set up correctly.
        /// </summary>
        [Fact]
        public async void GetByName_TestRequestSetup()
        {
            JToken actualRequest = null;
            JToken expectedRequest = JToken.Parse(@"
                {
                    ""sort"": [
                        {
                            ""term_name"": {}
                        }
                    ],
                    ""query"": {
                        ""bool"": {
                            ""must"": [
                                {
                                    ""term"": {
                                        ""language"": {
                                            ""value"": ""en""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""audience"": {
                                            ""value"": ""Patient""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""dictionary"": {
                                            ""value"": ""Cancer.gov""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""pretty_url_name"": {
                                            ""value"": ""s-1""
                                        }
                                    }
                                }
                            ]
                        }
                    }
                }"
            );

            Uri actualESURI = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetByName/s-1.json");
                res.StatusCode = 200;

                actualRequest = conn.GetRequestPost(req);
                actualESURI = req.Uri;
            });

            // Doesn't actually connect to the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            // Don't actually care that this returns anything, only that the connection set up the request correctly.
            await termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1");

            Assert.Equal(expectedRequest, actualRequest, new JTokenEqualityComparer());
            Assert.Equal("/glossaryv1/_search", actualESURI.AbsolutePath);
        }

        /// <summary>
        /// Test that GetByName throws exception for multiple results.
        /// </summary>
        [Theory]
        [InlineData("getbyname_multiplehits", "errors occured")]
        public async void GetByName_MultipleResults(string file, string expectedMessage)
        {
            IElasticClient client = GetByName_GetElasticClientWithData(file);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIInternalException ex = await Assert.ThrowsAsync<APIInternalException>(() => termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1"));
            Assert.Equal(expectedMessage, ex.Message);
        }

        /// <summary>
        /// Verify that GetByName returns in the expected manner when Elasticsearch reports that the
        /// term doesn't exist.
        /// </summary>
        [Fact]
        public async void GetByName_TermNotFound()
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes(
                    @"{
                        ""took"": 3,
                        ""timed_out"": false,
                        ""_shards"": {
                            ""total"": 1,
                            ""successful"": 1,
                            ""skipped"": 0,
                            ""failed"": 0
                        },
                        ""hits"": {
                            ""total"": {
                                ""value"": 0,
                                ""relation"": ""eq""
                            },
                            ""max_score"": null,
                            ""hits"": []
                        }
                    }"
                ),
                statusCode: 200,
                exception: null,
                contentType: "application/json"
            );
            // Doesn't actually connect to the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm result = await termsClient.GetByName("cancer.gov", AudienceType.Patient, "en", "s-1");

            Assert.Null(result);
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetByNameData))]
        public async void GetByName_DataLoading(GetByNameTermsQueryTestData data)
        {
            IElasticClient client = GetByName_GetElasticClientWithData(data.PrettyUrlName);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm glossaryTerm = await termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1");

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

        ///<summary>
        ///A private method to enrich data from file for GetById
        ///</summary>
        private IElasticClient GetByName_GetElasticClientWithData(string prettyUrlName)
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetByName/" + prettyUrlName + ".json");
                res.StatusCode = 200;
            });

            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            return client;
        }

    }
}