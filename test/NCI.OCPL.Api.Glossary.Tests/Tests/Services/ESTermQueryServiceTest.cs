using System;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Moq;
using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.BestBets.Tests.ESTermQueryTestData;
using Nest;
using Microsoft.Extensions.Logging.Testing;
using System.Collections.Generic;
using Xunit;
using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.BestBets.Tests;

namespace NCI.OCPL.Api.Glossary.Tests{
    public class ESTermQueryServiceTest{

        public static IEnumerable<object[]> JsonData => new[] {
            new object[] { new TermScenario_43966_NoMediaNoResources() },
            new object[] { new TermScenario_445043_ImageAndExternalLink() },
            new object[] { new TermScenario_44058_VideoExernalLink() },
            new object[] { new TermScenario_44759_NoMediaGlossaryResource() },
            new object[] { new TermScenario_44178_NoMediaSummaryLink() },
            new object[] { new TermScenario_44386_NoMediaDrugSummary() },
            new object[] { new TermScenario_339337_HealthProfessional() },
        };

        /// <summary>
        /// Test that URI for Elasticsearch is set up correctly.
        /// </summary>
        [Theory, MemberData(nameof(JsonData))]
        public async void GetByIdForGlossaryTerm(BaseTermQueryTestData data){
            Uri esURI = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermQueryData/" + data.ESTermID + ".json");

                res.StatusCode = 200;

                esURI = req.Uri;
            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermQueryService termClient = new ESTermQueryService(client, gTermClientOptions, new NullLogger<ESTermQueryService>());

            // We don't actually care that this returns anything - only that the intercepting connection
            // sets up the request URI correctly.
            GlossaryTerm actDisplay = await termClient.GetById(
                data.DictionaryName,
                data.Audience,
                data.Language,
                data.TermID,
                new string[]{}
            );


            Assert.Equal(
                esURI.Segments,
                new string[] { "/", "glossaryv1/", "terms/", data.ESTermID },
                new ArrayComparer()
            );
        }

        /// <summary>
        /// Test failure to connect to and retrieve response from API.
        /// </summary>
        [Fact]
        public async void GetByIdForGlossaryTerm_TestAPIConnectionFailure()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                res.StatusCode = 500;
            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermQueryService termClient = new ESTermQueryService(client, gTermClientOptions, new NullLogger<ESTermQueryService>());
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termClient.GetById("cancer.gov",AudienceType.Patient,"en",43966L,new string[]{}));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// This use case tests the scenario if ES has failed to connect or sends back
        /// invalid response.
        /// </summary>

        [Fact]
        public async void GetByIdForGlossaryTerm_TestInvalidResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {

            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermQueryService termClient = new ESTermQueryService(client, gTermClientOptions, new NullLogger<ESTermQueryService>());
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termClient.GetById("cancer.gov",AudienceType.Patient,"en",43966L,new string[]{}));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(JsonData))]
        public async void GetBestBetForDisplay_DataLoading(BaseTermQueryTestData data)
        {
            IElasticClient client = GetElasticClientWithData(data);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermClientOptions = GetMockOptions();

            ESTermQueryService termClient = new ESTermQueryService(client, gTermClientOptions, new NullLogger<ESTermQueryService>());

			GlossaryTerm glossaryTerm = await termClient.GetById("cancer.gov",AudienceType.Patient,"en",43966L,new string[]{});

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

        ///<summary>
        ///A private method to enrich data from file
        ///</summary>
        private IElasticClient GetElasticClientWithData(BaseTermQueryTestData data) {
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