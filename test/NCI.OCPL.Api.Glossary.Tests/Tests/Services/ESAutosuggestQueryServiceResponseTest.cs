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
    /// Tests for the Autosuggest Query Service, examining responses.
    /// </summary>
    public class ESAutosuggestQueryServiceResponseTest
    {
        /// <summary>
        /// Test data objects for testing "Begins with" suggestions (Contains = False.)
        /// </summary>
        public static IEnumerable<object[]> ResponseData => new[]
        {
            new object[] { new AutosuggestScenario_BeginAciSpanishPatient() },
            new object[] { new AutosuggestScenario_BeginDarEnglishPatient() },
            new object[] { new AutosuggestScenario_ContainCatEnglishPatient()},
            new object[] { new AutosuggestScenario_ContainCutaneoSpanishPatient() }
        };

        /// <summary>
        /// Test that the request to Elasticsearch goes to the correct URI
        /// and structures the request body as expected.
        /// </summary>
        [Theory, MemberData(nameof(ResponseData))]
        public async void GetSuggestions_TestBeginsRequestSetup(BaseAutosuggestTestData data)
        {
            Uri esURI = null;
            string esContentType = String.Empty;
            HttpMethod esMethod = HttpMethod.DELETE; // Basically, something other than the expected value.

            JObject requestBody = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<Suggestion>>((req, res) =>
            {
                res.Stream = TestingTools.GetTestFileAsStream("ESAutosuggestQueryResponse/" + data.TestFilename);
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

            // We don't really care about the inputs. What matters is the return, which is controlled by the mock ES connection.
            Suggestion[] result = await query.GetSuggestions("Cancer.gov", AudienceType.Patient, "es", "chicken", MatchType.Contains, 200);

            Assert.Equal(data.ExpectedData, result, new ArrayComparer<Suggestion, SuggestionComparer>());
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

    }
}