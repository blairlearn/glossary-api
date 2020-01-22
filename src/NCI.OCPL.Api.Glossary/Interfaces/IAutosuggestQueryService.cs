using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Interface for the service for working with
    /// auto suggestions
    /// </summary>
    public interface IAutosuggestQueryService
    {
        /// <summary>
        /// Retrieves a portion of the overall set of glossary terms for a given combination of dictionary, audience, and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="query">The search query</param>
        /// <param name="contains">Set to true to allow search to find terms which contain the query string instead of explicitly starting with it.</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <returns>An array of Suggestion objects</returns>
        Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string query, bool contains, int size, int from);
    }

}