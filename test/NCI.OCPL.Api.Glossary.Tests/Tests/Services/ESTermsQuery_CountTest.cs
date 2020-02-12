using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elasticsearch.Net;
using Moq;
using Nest;
using Newtonsoft.Json.Linq;
using Xunit;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Tests to verify that the requests for the term count
    /// are put together in the expected manner.
    /// </summary>
    public class ESTermsQuery_CountTest
    {
        /// <summary>
        /// Test data objects for use when validating how requests to Elasticsearch are assembled.
        /// </summary>
        public static IEnumerable<object[]> RequestData => new[]
        {
            new object[] {new Terms_GetCount_Request_CGov_HealthProfessional_English() },
            new object[] {new Terms_GetCount_Request_CGov_HealthProfessional_Spanish() },
            new object[] {new Terms_GetCount_Request_CGov_Patient_English() },
            new object[] {new Terms_GetCount_Request_CGov_Patient_Spanish() },
            new object[] {new Terms_GetCount_Request_Genetics_HealthProfessional_English() },
            new object[] {new Terms_GetCount_Request_Genetics_HealthProfessional_Spanish() },
            new object[] {new Terms_GetCount_Request_Genetics_Patient_English() },
            new object[] {new Terms_GetCount_Request_Genetics_Patient_Spanish() }
        };

        /// <summary>
        /// Test data objects for use when validating how Elasticsearch responses are handled.
        /// </summary>
        public static IEnumerable<object[]> ResponseData => new[]
        {
            new object[] { new Terms_GetCount_Response_Zero() },
            new object[] { new Terms_GetCount_Response_One() },
            new object[] { new Terms_GetCount_Response_Realistic() },
            new object[] { new Terms_GetCount_Response_OMG() }
        };

        /// <summary>
        /// Verify the ES request to get the count is put together correctly.
        /// </summary>
        /// <param name="data"></param>
        [Theory, MemberData(nameof(RequestData))]
        public async void GetCount_Request(BaseTermsQueryCountTestData data)
        {
            Uri esURI = null;
            string esContentType = String.Empty;
            HttpMethod esMethod = HttpMethod.DELETE; // Basically, something other than the expected value.

            JObject requestBody = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.CountResponse>((req, res) =>
            {
                // We don't really care about the response for this test.
                res.Stream = GetMockCountResponse();
                res.StatusCode = 200;

                esURI = req.Uri;
                esContentType = req.ContentType;
                esMethod = req.Method;
                requestBody = conn.GetRequestPost(req);
            });

            // The URI does not matter, an InMemoryConnection never requests from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't really care that this returns anything (for this test), only that the intercepting connection
            // sets up the request correctly.
            long result = await query.GetCount(data.DictionaryName, data.Audience, data.Language);

            Assert.Equal("/glossaryv1/terms/_count", esURI.AbsolutePath);
            Assert.Equal("application/json", esContentType);
            Assert.Equal(HttpMethod.POST, esMethod);
            Assert.Equal(data.ExpectedData, requestBody, new JTokenEqualityComparer());
        }


        /// <summary>
        /// Verify the response from ES is processed correctly.
        /// </summary>
        [Theory, MemberData(nameof(ResponseData))]
        public async void GetCount_Response(BaseTermsCountResponseData data)
        {
            Uri esURI = null;
            string esContentType = String.Empty;
            HttpMethod esMethod = HttpMethod.DELETE; // Basically, something other than the expected value.

            JObject requestBody = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.CountResponse>((req, res) =>
            {
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetCount/" + data.TestFilename);
                res.StatusCode = 200;

                esURI = req.Uri;
                esContentType = req.ContentType;
                esMethod = req.Method;
                requestBody = conn.GetRequestPost(req);
            });

            // The URI does not matter, an InMemoryConnection never requests from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't really care about the inputs. What matters is the return, which is controlled by the mock ES connection.
            long result = await query.GetCount("Cancer.gov", AudienceType.Patient, "es");

            Assert.Equal(data.ExpectedCount, result);
        }

        /// <summary>
        /// Verify that ESTermsQueryService responds correctly when Elasticsearch returns an error.
        /// </summary>
        [Fact]
        public async void GetCount_ErrorResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.CountResponse>((req, res) =>
            {
                res.Stream = null;
                res.StatusCode = 200;
            });

            // The URI does not matter, an InMemoryConnection never requests from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetCount("Cancer.gov", AudienceType.HealthProfessional, "es")
            );
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify that ESTermsQueryService responds correctly if Elasticsearch returns
        /// a broken response.
        /// </summary>
        [Fact]
        public async void GetCount_InvalidResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.CountResponse>((req, res) =>
            {
                string partial =@"{
                    ""count"": 8458,
                    ""_shards"": {";
                byte[] byteArray = Encoding.UTF8.GetBytes(partial);
                res.Stream = new MemoryStream(byteArray);
                res.StatusCode = 200;
            });

            // The URI does not matter, an InMemoryConnection never requests from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetCount("Cancer.gov", AudienceType.HealthProfessional, "es")
            );
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Mock Elasticsearch configuraiton options.
        /// </summary>
        private IOptions<GlossaryAPIOptions> GetMockOptions()
        {
            Mock<IOptions<GlossaryAPIOptions>> clientOptions = new Mock<IOptions<GlossaryAPIOptions>>();
            clientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new GlossaryAPIOptions()
                {
                    AliasName = "glossaryv1"
                }
            );

            return clientOptions.Object;
        }

        /// <summary>
        /// Simulates a count response from Elasticsearch so we
        /// have something for tests where we don't care about the response.
        /// </summary>
        private Stream GetMockCountResponse()
        {
            string empty = @"
{
    ""count"": 42,
    ""_shards"": {
                ""total"": 1,
        ""successful"": 1,
        ""skipped"": 0,
        ""failed"": 0
    }
        }";
            byte[] byteArray = Encoding.UTF8.GetBytes(empty);
            return new MemoryStream(byteArray);
        }
    }
}