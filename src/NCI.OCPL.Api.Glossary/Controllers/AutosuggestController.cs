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
        public async Task<Suggestion[]> GetSuggestions(string dictionary, string audience, string language, string query){
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || String.IsNullOrWhiteSpace(audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(404, "Unsupported Language. Please try either 'en' or 'es'");

            AudienceType audienceType;
            if(!Enum.TryParse(audience,true,out audienceType))
                    throw new APIErrorException(400, "'AudienceType' can  be 'Patient' or 'HealthProfessional' only");

            return await _autosuggestQueryService.GetSuggestions(dictionary, audienceType, language, query);
        }
    }
}