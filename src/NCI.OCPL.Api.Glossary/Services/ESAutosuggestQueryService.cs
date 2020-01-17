using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace NCI.OCPL.Api.Glossary.Services
{
    /// <summary>
    /// Elasticsearch implementation of the service for retrieveing suggestions for
    /// GlossaryTerm objects.
    /// </summary>
    public class ESAutosuggestQueryService : IAutosuggestQueryService
    {

        private IElasticClient _elasticClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ESAutosuggestQueryService(IElasticClient client)
        {
            _elasticClient = client;
        }

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The search query</param>
        /// <returns>A list of GlossaryTerm</returns>
        /// </summary>
        public async Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string query)
        {
            // Temporary Solution till we have Elastic Search
            List<Suggestion> suggestionList = new List<Suggestion>();
            suggestionList.AddRange(GenerateSampleTerms());

            return suggestionList.ToArray();
        }

        private Suggestion[] GenerateSampleTerms(){

            Suggestion[] results = new Suggestion[]{

                new Suggestion(){
                    TermId = 123,
                    TermName = "suggestion 1"
                },
                new Suggestion(){
                    TermId = 456,
                    TermName = "suggestion 2"
                },
                new Suggestion(){
                    TermId = 789,
                    TermName = "suggestion 3"
                }

            };

            return results;
        }

    }
}