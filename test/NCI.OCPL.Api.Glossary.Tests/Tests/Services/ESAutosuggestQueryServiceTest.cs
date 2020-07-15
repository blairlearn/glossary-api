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

using NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Tests for the Autosuggest Query Service.
    /// </summary>
    public class ESAutosuggestQueryServiceTest
    {
        /// <summary>
        /// Test data objects for testing "Begins with" suggestions (Contains = False.)
        /// </summary>
        public static IEnumerable<object[]> RequestBeginData => new[]
        {
            new object[] { new Autosuggest_Request_Genetics_Begin_Dar_English_HealthProfessional() },
            new object[] { new Autosuggest_Request_CGov_Begin_Aci_Spanish_Patient() },
            new object[] { new Autosuggest_Request_CGov_Contains_Cat_English_Patient() },
            new object[] { new Autosuggest_Request_Genetics_Contains_Gene_English_HealthProfessional() },
            new object[] { new Autosuggest_Request_CGov_Contains_Ablacion_Spanish_Patient() },
            new object[] { new Autosuggest_Request_Exact() }
        };

        /// <summary>
        /// Test that the request to Elasticsearch goes to the correct URI
        /// and structures the request body as expected.
        /// </summary>
        [Theory, MemberData(nameof(RequestBeginData))]
        public async void GetSuggestions_TestBeginsRequestSetup(BaseAutosuggestRequestTestData data)
        {
            Uri esURI = null;
            string esContentType = String.Empty;
            HttpMethod esMethod = HttpMethod.DELETE; // Basically, something other than the expected value.

            JObject requestBody = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<Suggestion>>((req, res) =>
            {
                // We don't really care about the response for this test.
                res.Stream = GetMockEmptyResponse();
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

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't really care that this returns anything (for this test), only that the intercepting connection
            // sets up the request correctly.
            Suggestion[] result = await query.GetSuggestions(data.DictionaryName, data.Audience, data.Language, data.SearchText, data.MatchType, data.Size);

            Assert.Equal("/glossaryv1/terms/_search", esURI.AbsolutePath);
            Assert.Equal("application/json", esContentType);
            Assert.Equal(HttpMethod.POST, esMethod);
            Assert.Equal(data.ExpectedData, requestBody, new JTokenEqualityComparer());
        }


        /// <summary>
        /// Test that the ESTermsQueryService responds correctly when ES returns an empty result set.
        /// </summary>
        [Fact]
        public async void GetSuggestions_TestEmptyESResults()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<Suggestion>>((req, res) =>
            {
                // We don't really care about the response for this test.
                res.Stream = GetMockEmptyResponse();
                res.StatusCode = 200;
            });

            // The URI does not matter, an InMemoryConnection never requests from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't really care about the inputs, only the result.
            Suggestion[] result = await query.GetSuggestions("Cancer.gov", AudienceType.HealthProfessional, "es", "chicken", MatchType.Contains, 200);

            Assert.Empty(result);
        }

        /// <summary>
        /// Verify that ESTermsQueryService responds correctly when Elasticsearch returns an error.
        /// </summary>
        [Fact]
        public async void GetSuggestions_TestErrorResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<Suggestion>>((req, res) =>
            {
                // Simulate an error.
                res.Stream = null;
                res.StatusCode = 500;
            });

            // The URI does not matter, an InMemoryConnection never requests from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetSuggestions("Cancer.gov", AudienceType.HealthProfessional, "es", "chicken", MatchType.Contains, 200)
            );
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify that ESTermsQueryService responds correctly if Elasticsearch returns a broken
        /// response.
        /// </summary>
        [Fact]
        public async void GetSuggestions_TestInvalidResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<Suggestion>>((req, res) =>
            {
                string partial = @"{
                    ""took"": 223,
                    ""timed_out"": false,
                    ""_shards"": {
                                ""total"": 1,
                        ""successful"": 1,";
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

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetSuggestions("Cancer.gov", AudienceType.HealthProfessional, "es", "chicken", MatchType.Contains, 200)
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
        /// Simulates a "no results found" response from Elasticsearch so we
        /// have something for tests where we don't care about the response.
        /// </summary>
        private Stream GetMockEmptyResponse()
        {
            string empty = @"
{
    ""took"": 223,
    ""timed_out"": false,
    ""_shards"": {
                ""total"": 1,
        ""successful"": 1,
        ""skipped"": 0,
        ""failed"": 0
    },
    ""hits"": {
                ""total"": 0,
        ""max_score"": null,
        ""hits"": []
    }
}";
            byte[] byteArray = Encoding.UTF8.GetBytes(empty);
            return new MemoryStream(byteArray);
        }

    }
}