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
using Xunit;
using NCI.OCPL.Api.Common;
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
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermClientOptions, new NullLogger<ESTermsQueryService>());

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
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermClientOptions, new NullLogger<ESTermsQueryService>());

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
                res.Stream = TestingTools.GetTestFileAsStream("ESTermQueryData/" + data.ESTermID + ".json");

                res.StatusCode = 200;

                esURI = req.Uri;
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermsQueryService termClient = new ESTermsQueryService(client, gTermClientOptions, new NullLogger<ESTermsQueryService>());

            // We don't actually care that this returns anything - only that the intercepting connection
            // sets up the request URI correctly.
            GlossaryTerm actDisplay = await termClient.GetById(
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
            IElasticClient client = GetElasticClientWithData(data);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermsQueryService termClient = new ESTermsQueryService(client, gTermClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm glossaryTerm = await termClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L, new string[] { });

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

        #endregion

        ///<summary>
        ///A private method to enrich data from file
        ///</summary>
        private IElasticClient GetElasticClientWithData(BaseTermsQueryTestData data)
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermQueryData/" + data.ESTermID + ".json");

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