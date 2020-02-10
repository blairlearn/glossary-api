using System;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Moq;
using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;
using Nest;
using Microsoft.Extensions.Logging.Testing;
using System.Collections.Generic;
using System.IO;
using Xunit;
using NCI.OCPL.Api.Common;
using Newtonsoft.Json.Linq;
using NCI.OCPL.Api.Glossary.Tests;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public class ESTermsQueryServiceTest
    {

        public static IEnumerable<object[]> GetByIdData => new[] {
            new object[] { new GetById_43966_NoMediaNoResources() },
            new object[] { new GetById_44058_VideoExernalLink() },
            new object[] { new GetById_44759_NoMediaGlossaryResource() },
            new object[] { new GetById_44178_NoMediaSummaryLink() },
            new object[] { new GetById_44386_NoMediaDrugSummary() },
            new object[] { new GetById_339337_HealthProfessional() },
            new object[] { new GetById_445043_ImageAndExternalLink() },
        };

        public static IEnumerable<object[]> ExpandData => new[] {
            new object[] { new Expand_S() },
            new object[] { new Expand_NoResults() }
        };

        #region GetById tests

        /// <summary>
        /// Test failure to connect to and retrieve response from API in GetById.
        /// </summary>
        [Fact]
        public async void GetById_TestAPIConnectionFailure()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                res.StatusCode = 500;
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L, new string[] { }));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test failure to connect to ES or receiving an invalid response from ES in GetById.
        /// </summary>
        [Fact]
        public async void GetById_TestInvalidResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {

            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L, new string[] { }));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test that GetById URI for Elasticsearch is set up correctly.
        /// </summary>
        [Theory, MemberData(nameof(GetByIdData))]
        public async void GetById_TestUriSetup(BaseTermsQueryTestData data)
        {
            Uri esURI = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetById/" + data.ESTermID + ".json");

                res.StatusCode = 200;

                esURI = req.Uri;
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            // We don't actually care that this returns anything - only that the intercepting connection
            // sets up the request URI correctly.
            GlossaryTerm actDisplay = await termsClient.GetById(
                data.DictionaryName,
                data.Audience,
                data.Language,
                data.TermID,
                new string[] { }
            );

            Assert.Equal( $"/glossaryv1/terms/{data.ESTermID}", esURI.AbsolutePath);
        }

        /// <summary>
        /// Tests the correct loading of various data files for GetById.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetByIdData))]
        public async void GetById_DataLoading(BaseTermsQueryTestData data)
        {
            IElasticClient client = GetById_GetElasticClientWithData(data);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm glossaryTerm = await termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L, new string[] { });

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

        #endregion


        #region Expand tests

        /// <summary>
        /// Test failure to connect to and retrieve response from API in GetById
        /// </summary>
        [Fact]
        public async void Expand_TestAPIConnectionFailure()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                res.StatusCode = 500;
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "a", 10, 0, new string[] { }));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test failure to connect to ES or receiving an invalid response from ES in GetById.
        /// </summary>
        [Fact]
        public async void Expand_TestInvalidResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {

            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "a", 10, 0, new string[] { }));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test that Expand Request for Elasticsearch is set up correctly.
        /// </summary>
        [Fact]
        public async void Expand_TestRequestSetup()
        {
            JObject actualRequest = null;
            JObject expectedRequest = JObject.Parse(@"
                {
                    ""from"": 0,
                    ""size"": 5,
                    ""_source"": {
                        ""includes"": [
                            ""term_id"",
                            ""language"",
                            ""dictionary"",
                            ""audience"",
                            ""term_name"",
                            ""first_letter"",
                            ""pretty_url_name"",
                            ""definition"",
                            ""pronunciation""
                        ]
                    },
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
                                        ""first_letter"": {
                                            ""value"": ""s""
                                        }
                                    }
                                }
                            ]
                        }
                    }
                }"
            );

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/Expand/expand_response_results.json");

                res.StatusCode = 200;

                actualRequest = conn.GetRequestPost(req);
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            try
            {
                var results = await termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 5, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"});
            }
            catch (Exception) { }

            Assert.Equal(expectedRequest, actualRequest, new JTokenEqualityComparer());
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(ExpandData))]
        public async void Expand_DataLoading(ExpandTermsQueryTestData data)
        {
            IElasticClient client = Expand_GetElasticClientWithData(data.ExpandTestType);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTermResults glossaryTermResults = await termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 5, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"});

            Assert.Equal(data.ExpectedData.Results, glossaryTermResults.Results, new GlossaryTermComparer());
            Assert.Equal(data.ExpectedData.Meta.TotalResults, glossaryTermResults.Meta.TotalResults);
            Assert.Equal(data.ExpectedData.Meta.From, glossaryTermResults.Meta.From);
        }

        #endregion

        ///<summary>
        ///A private method to enrich data from file for GetById
        ///</summary>
        private IElasticClient GetById_GetElasticClientWithData(BaseTermsQueryTestData data)
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetById/" + data.ESTermID + ".json");

                res.StatusCode = 200;
            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            return client;
        }

        ///<summary>
        ///A private method to enrich data from file for GetById
        ///</summary>
        private IElasticClient Expand_GetElasticClientWithData(string expandTestType)
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/Expand/expand_response_" + expandTestType + ".json");

                res.StatusCode = 200;
            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            return client;
        }

        ///<summary>
        ///A private method to enrich IOptions
        ///</summary>
        private IOptions<GlossaryAPIOptions> GetMockOptions()
        {
            Mock<IOptions<GlossaryAPIOptions>> glossaryAPIClientOptions = new Mock<IOptions<GlossaryAPIOptions>>();
            glossaryAPIClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new GlossaryAPIOptions()
                {
                    AliasName = "glossaryv1"
                }
            );

            return glossaryAPIClientOptions.Object;
        }
    }
}