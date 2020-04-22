using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NCI.OCPL.Api.Common;

namespace NCI.OCPL.Api.Glossary.Controllers
{
    /// <summary>
    /// Controller for routes used when autosuggesting
    /// multiple Terms.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AutosuggestController : Controller
    {
        private readonly IAutosuggestQueryService _autosuggestQueryService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AutosuggestController(IAutosuggestQueryService service)
        {
            _autosuggestQueryService = service;
        }

        /// <summary>
        /// Search for Terms based on autosuggest criteria
        /// </summary>
        /// <returns>An array GlossaryTerm objects</returns>

        /// <summary>
        /// Searches for dictionary terms with names matching the query text.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="searchText">Text to match against</param>
        /// <param name="matchType">Should the search match items beginning with the search text, or containing it?</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <returns></returns>
        [HttpGet("{dictionary:required}/{audience:required}/{language:required}/{searchText:required}")]
        public async Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string searchText,
            [FromQuery] MatchType matchType = MatchType.Begins, [FromQuery] int size = 20)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language.");

            if(!Enum.IsDefined(typeof(MatchType), matchType))
                throw new APIErrorException(400, "The `matchType` parameter must be either 'Begins' or 'Contains'.");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(400, "Unsupported Language. Valid values are 'en' and 'es'.");

            if (size <= 0)
                size = 20;

            return await _autosuggestQueryService.GetSuggestions(dictionary, audience, language, searchText, matchType, size);
        }
    }
}