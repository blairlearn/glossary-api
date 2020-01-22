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
        /// </summary>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The search query</param>
        /// <param name="contains">Set to true to allow search to find terms which contain the query string instead of explicitly starting with it.</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <returns>An array of Suggestion objects</returns>
        public async Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string query, bool contains, int size, int from)
        {
            return new Suggestion[]{

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
        }

    }
}