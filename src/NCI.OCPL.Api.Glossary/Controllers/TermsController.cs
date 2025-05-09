using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Common;
using System.Collections.Generic;
using System.Linq;

namespace NCI.OCPL.Api.Glossary.Controllers
{

    /// <summary>
    /// Controller for routes used when searching for or retrieving
    /// multiple Terms.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TermsController : Controller
    {
        // Set dictionary name constants
        const string CANCER_GOV_DICTIONARY = "cancer.gov";
        const string GENETICS_DICTIONARY = "genetics";
        const string NOT_SET_DICTIONARY = "notset";

        private readonly ILogger _logger;
        private readonly ITermsQueryService _termsQueryService;

        // Set Patient fallback option combinations
        private readonly LinkedList<Tuple<string, AudienceType>> _fallbackOptionsPatient = new LinkedList<Tuple<string, AudienceType>>(
            new Tuple<string, AudienceType>[]
            {
                new Tuple<string, AudienceType>(CANCER_GOV_DICTIONARY, AudienceType.Patient),
                new Tuple<string, AudienceType>(CANCER_GOV_DICTIONARY, AudienceType.HealthProfessional),
                new Tuple<string, AudienceType>(NOT_SET_DICTIONARY, AudienceType.Patient),
                new Tuple<string, AudienceType>(NOT_SET_DICTIONARY, AudienceType.HealthProfessional),
                new Tuple<string, AudienceType>(GENETICS_DICTIONARY, AudienceType.Patient),
                new Tuple<string, AudienceType>(GENETICS_DICTIONARY, AudienceType.HealthProfessional)
            }
        );

        // Set HealthProfessional fallback option combinations
        private readonly LinkedList<Tuple<string, AudienceType>> _fallbackOptionsHP = new LinkedList<Tuple<string, AudienceType>>(
            new Tuple<string, AudienceType>[]
            {
                new Tuple<string, AudienceType>(GENETICS_DICTIONARY, AudienceType.HealthProfessional),
                new Tuple<string, AudienceType>(GENETICS_DICTIONARY, AudienceType.Patient),
                new Tuple<string, AudienceType>(CANCER_GOV_DICTIONARY, AudienceType.HealthProfessional),
                new Tuple<string, AudienceType>(CANCER_GOV_DICTIONARY, AudienceType.Patient),
                new Tuple<string, AudienceType>(NOT_SET_DICTIONARY, AudienceType.HealthProfessional),
                new Tuple<string, AudienceType>(NOT_SET_DICTIONARY, AudienceType.Patient)
            }
        );

        /// <summary>
        /// Constructor.
        /// </summary>
        public TermsController(ILogger<TermsController> logger, ITermsQueryService service)
        {
            _logger = logger;
            _termsQueryService = service;
        }

        /// <summary>
        /// Retrieve the details of a Glossary Term by ID value.
        /// </summary>
        /// <param name="dictionary">The dictionary to use. Valid values are "cancer.gov" for the Dictionary of Cancer Terms
        /// and "genetics" for the Dictionary of Genetics Terms.</param>
        /// <param name="audience">The intended audience.</param>
        /// <param name="language">The language in which to fetch the details. Valid values are "en" for English and "es" for Spanish.</param>
        /// <param name="id">The term's ID</param>
        /// <param name="useFallback">Set true to use falback logic if the term isn't available for the specified combination of audience and dictionary.</param>
        /// <returns>GlossaryTerm object</returns>
        [HttpGet("{dictionary:required}/{audience:required}/{language:required}/{id:long}")]
        public async Task<GlossaryTerm> GetById(string dictionary, AudienceType audience, string language, long id, bool useFallback = false)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || id <= 0)
            {
                throw new APIErrorException(400, "You must supply a valid dictionary, audience, language and id");
            }

            // Lowercase the dictionary argument for use in fallback option checks and ES query
            dictionary = dictionary.ToLower();

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(400, "Unsupported Language. Please try either 'en' or 'es'");

            if (useFallback == false)
            {
                GlossaryTerm result = null;
                try
                {
                    result = await _termsQueryService.GetById(dictionary, audience, language, id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error encountered for dictionary '{dictionary}', audience '{audience}', language '{language}' and id '{id}.");
                    throw new APIErrorException(500, "Errors Occured");
                }
                if(result != null)
                    return result;
                else
                    throw new APIErrorException(404, $"No match for dictionary '{dictionary}', audience '{audience}', language '{language}' and id '{id}'.");
            }
            // Implement fallback logic.
            // Depending on the given Dictionary and Audience inputs, loop through the fallback logic options until a term is found.
            // If the given fallback combination does not exist, then throw an error.
            // If none of the options return a term, then throw an error.
            else
            {
                // Set order of fallback options based on current audience.
                var fallbackOptions = audience == AudienceType.Patient ? _fallbackOptionsPatient : _fallbackOptionsHP;

                Tuple<string, AudienceType> requestedOptions = new Tuple<string, AudienceType>(dictionary, audience);
                LinkedListNode<Tuple<string, AudienceType>> start = fallbackOptions.Find(requestedOptions);

                if (start == null)
                {
                    string msg = $"Could not find initial fallback combination with dictionary '{dictionary}' and audience '{audience}'.";
                    throw new APIErrorException(404, msg);
                }

                LinkedListNode<Tuple<string, AudienceType>> current = start;

                do
                {
                    try
                    {
                        GlossaryTerm result = await _termsQueryService.GetById(current.Value.Item1, current.Value.Item2, language, id);
                        if(result == null)
                        {
                            current = current.Next == null ? fallbackOptions.First : current.Next;
                            continue;
                        }
                        else
                            return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error encountered for dictionary '{current.Value.Item1}', audience '{current.Value.Item2}', language '{language}' and id '{id}.");
                        throw new APIErrorException(500, "Errors Occured");
                    }
                } while ( current != start );

                string message = $"Could not find fallback term with ID '{id}' for any combination of dictionary and audience.";
                throw new APIErrorException(404, message);
            }
        }

        /// <summary>
        /// Get the Glossary Term based on Pretty URL Name.
        /// </summary>
        /// <param name="dictionary">The dictionary to use. Valid values are "cancer.gov" for the Dictionary of Cancer Terms
        /// and "genetics" for the Dictionary of Genetics Terms.</param>
        /// <param name="audience">The intended audience.</param>
        /// <param name="language">The language in which to fetch the details. Valid values are "en" for English and "es" for Spanish.</param>
        /// <param name="prettyUrlName">The term's name as a path segment.</param>
        /// <param name="useFallback">Set true to use falback logic if the term isn't available for the specified combination of audience and dictionary.</param>
        /// <returns>GlossaryTerm object</returns>
        [HttpGet("{dictionary:required}/{audience:required}/{language:required}/{prettyUrlName}")]
        public async Task<GlossaryTerm> GetByName(string dictionary, AudienceType audience, string language, string prettyUrlName, bool useFallback = false)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
            {
                throw new APIErrorException(400, "You must supply a valid dictionary, audience, and language.");
            }

            // Lowercase the dictionary argument for use in fallback option checks and ES query
            dictionary = dictionary.ToLower();

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(400, "Unsupported Language. Please try either 'en' or 'es'");

            if (String.IsNullOrWhiteSpace(prettyUrlName))
                throw new APIErrorException(400, "You must specify the prettyUrlName parameter.");

            // Call GetByName with specified parameters.
            if (useFallback == false)
            {
                GlossaryTerm result = null;
                try
                {
                    result = await _termsQueryService.GetByName(dictionary, audience, language, prettyUrlName);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Error encountered for dictionary '{dictionary}', audience '{audience}', language '{language}' and pretty url '{prettyUrlName}'.");
                    throw new APIErrorException(500, "Errors occured");
                }
                if(result != null)
                    return result;
                else
                    throw new APIErrorException(404, $"No match for dictionary '{dictionary}', audience '{audience}', language '{language}' and pretty url '{prettyUrlName}'.");
            }
            // Implement fallback logic.
            // Depending on the given Dictionary and Audience inputs, loop through the fallback logic options until a term is found.
            // If the given fallback combination does not exist, then throw an error.
            // If none of the options return a term, then throw an error.
            else
            {
                // Set order of fallback options based on current audience.
                var fallbackOptions = audience == AudienceType.Patient ? _fallbackOptionsPatient : _fallbackOptionsHP;

                Tuple<string, AudienceType> requestedOptions = new Tuple<string, AudienceType>(dictionary, audience);
                LinkedListNode<Tuple<string, AudienceType>> start = fallbackOptions.Find(requestedOptions);

                if (start == null)
                {
                    string msg = $"Could not find initial fallback combination with dictionary '{dictionary}' and audience '{audience}'.";
                    throw new APIErrorException(404, msg);
                }

                LinkedListNode<Tuple<string, AudienceType>> current = start;

                do
                {
                    try
                    {
                        GlossaryTerm result = await _termsQueryService.GetByName(current.Value.Item1, current.Value.Item2, language, prettyUrlName);
                        if(result == null)
                        {
                            current = current.Next == null ? fallbackOptions.First : current.Next;
                            continue;
                        }
                        else
                            return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error encountered for dictionary '{current.Value.Item1}', audience '{current.Value.Item2}', language '{language}' and pretty URL name '{prettyUrlName}'.");
                        throw new APIErrorException(500, "Errors Occured");
                    }

                } while ( current != start );

                string message = $"Could not find fallback term with pretty URL name '{prettyUrlName}' for any combination of dictionary and audience.";
                throw new APIErrorException(404, message);
            }
        }

        /// <summary>
        /// Retrieves a portion of the overall set of glossary terms for a given combination of dictionary, audience, and language.
        /// </summary>
        /// <param name="dictionary">The dictionary to use. Valid values are "cancer.gov" for the Dictionary of Cancer Terms
        /// and "genetics" for the Dictionary of Genetics Terms.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="includeAdditionalInfo">If true, the RelatedResources and Media fields will be populated. Else, they will be empty.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        [HttpGet("{dictionary:required}/{audience:required}/{language:required}")]
        public async Task<GlossaryTermResults> GetAll(string dictionary, AudienceType audience, string language, int size = 100, int from = 0, bool includeAdditionalInfo = false)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language.");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(400, "Unsupported Language. Valid values are 'en' and 'es'.");

            if (size <= 0)
                size = 100;

            if (from < 0)
                from = 0;

            GlossaryTermResults res = await _termsQueryService.GetAll(dictionary, audience, language, size, from, includeAdditionalInfo);

            return res;
        }

        /// <summary>
        /// Search for Terms based on search criteria
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="query">The string to search for.</param>
        /// <param name="matchType">Should the search match items beginning with the search text (Begins),
        /// containing it (Contains), or an exact match (Exact)?</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="includeAdditionalInfo">If true, the RelatedResources and Media fields will be populated. Else, they will be empty.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        [HttpGet("search/{dictionary:required}/{audience:required}/{language:required}/{*query:required}")]
        public async Task<GlossaryTermResults> Search(string dictionary, AudienceType audience, string language, string query,
            [FromQuery] MatchType matchType = MatchType.Begins, [FromQuery] int size = 100, [FromQuery] int from = 0, [FromQuery] bool includeAdditionalInfo = false)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language.");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(400, "Unsupported Language. Valid values are 'en' and 'es'.");

            if (!Enum.IsDefined(typeof(MatchType), matchType))
                throw new APIErrorException(400, "Invalid value for the 'matchType' parameter.");

            if (size <= 0)
                size = 100;

            if (from < 0)
                from = 0;

            // query uses a catch-all route, make sure it's been decoded.
            query = WebUtility.UrlDecode(query);

            GlossaryTermResults res = await _termsQueryService.Search(dictionary, audience, language, query, matchType, size, from, includeAdditionalInfo);
            return res;
        }

        /// <summary>
        /// Get all Terms starting with the character passed.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="character">The character to search the query</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="includeAdditionalInfo">If true, the RelatedResources and Media fields will be populated. Else, they will be empty.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        [HttpGet("expand/{dictionary}/{audience}/{language}/{character}")]
        public async Task<GlossaryTermResults> Expand(string dictionary, AudienceType audience, string language, string character,
            [FromQuery] int size = 100, [FromQuery] int from = 0, [FromQuery] bool includeAdditionalInfo = false)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(400, "Unsupported Language. Please try either 'en' or 'es'");

            if (size <= 0)
                size = 100;

            if (from < 0)
                from = 0;

            GlossaryTermResults res = await _termsQueryService.Expand(dictionary, audience, language, character, size, from, includeAdditionalInfo);
            return res;
        }

        /// <summary>
        /// Get the total number of terms available in the version of a dictionary matching a specific audience and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <returns>The number of terms available.</returns>
        [HttpGet("count/{dictionary:required}/{audience:required}/{language:required}")]
        public async Task<long> GetCount(string dictionary, AudienceType audience, string language)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || !Enum.IsDefined(typeof(AudienceType), audience))
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language.");

            return await _termsQueryService.GetCount(dictionary, audience, language);
        }
    }
}
