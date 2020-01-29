using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Interface for the service for working with
    /// multiple Terms.
    /// </summary>
    public interface ITermsQueryService
    {
/// <summary>
        /// Get Term details based on the input values
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="id">The Id for the term</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>An object of GlossaryTerm</returns>
        /// </summary>
        Task<GlossaryTerm> GetById(string dictionary, AudienceType audience, string language, long id, string[] requestedFields);

        /// <summary>
        /// Retrieves a portion of the overall set of glossary terms for a given combination of dictionary, audience, and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="requestedFields">The fields to retrieve.  If not specified, defaults to TermName, Pronunciation, and Definition.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        Task<GlossaryTermResults> GetAll(string dictionary, AudienceType audience, string language, int size, int from, string[] requestedFields);

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The search query</param>
        /// <param name="matchType">Defines if the search should begin with or contain the key word</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>A list of GlossaryTerm</returns>
        /// </summary>
        Task<List<GlossaryTerm>> Search(string dictionary, AudienceType audience, string language, string query,string matchType, int size, int from, string[] requestedFields);

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="expandCharacter">The character to search the query</param>
        /// <param name="matchType">Defines if the search should begin with or contain the key word</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>A list of GlossaryTerm</returns>
        /// </summary>
        Task<List<GlossaryTerm>> Expand(string dictionary, AudienceType audience, string language, string expandCharacter,string matchType, int size, int from, string[] requestedFields);
    }
}
