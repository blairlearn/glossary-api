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
        [HttpGet("/{dictionary}/{audience}/{language}/{query}")]
        public async Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string query,
            [FromQuery] bool contains = false, [FromQuery] int size = 20, [FromQuery] int from = 0)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language.");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(404, "Unsupported Language. Valid values are 'en' and 'es'.");

            if (size <= 0)
                size = 20;

            if (from < 0)
                from = 0;


            return await _autosuggestQueryService.GetSuggestions(dictionary, audience, language, query, contains, size, from);
        }
    }
}