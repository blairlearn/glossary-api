using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary.Models;
using Nest;

namespace NCI.OCPL.Api.Glossary.Services
{

    /// <summary>
    /// Elasticsearch implementation of the service for retrieveing multiple
    /// GlossaryTerm objects.
    /// </summary>
    public class ESTermsQueryService : ITermsQueryService
    {
        /// <summary>
        /// The elasticsearch client
        /// </summary>
        private IElasticClient _elasticClient;

        /// <summary>
        /// The API options.
        /// </summary>
        protected readonly GlossaryAPIOptions _apiOptions;

        /// <summary>
        /// A logger to use for logging
        /// </summary>
        private readonly ILogger<ESTermsQueryService> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ESTermsQueryService(IElasticClient client,IOptions<GlossaryAPIOptions> apiOptionsAccessor,
            ILogger<ESTermsQueryService> logger)
        {
            _elasticClient = client;
            _apiOptions = apiOptionsAccessor.Value;
            _logger = logger;
        }

        /// <summary>
        /// Get Term details based on the input values
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="id">The Id for the term</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>An object of GlossaryTerm</returns>
        /// </summary>
        public async Task<GlossaryTerm> GetById(string dictionary, AudienceType audience, string language, long id, string[] requestedFields)
        {
            IGetResponse<GlossaryTerm> response = null;

            try
            {
                string idValue = $"{id}_{dictionary?.ToLower()}_{language?.ToLower()}_{audience.ToString().ToLower()}";
                response = await _elasticClient.GetAsync<GlossaryTerm>(new DocumentPath<GlossaryTerm>(idValue),
                        g => g.Index( this._apiOptions.AliasName ).Type("terms"));

            }
            catch (Exception ex)
            {
                String msg = String.Format("Could not search dictionary '{0}', audience '{1}', language '{2}' and id '{3}.", dictionary, audience, language, id);
                _logger.LogError(msg, ex);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = String.Format("Invalid response when searching for dictionary '{0}', audience '{1}', language '{2}' and id '{3}.", dictionary, audience, language, id);
                _logger.LogError(msg);
                throw new APIErrorException(500, msg);
            }

            if(null==response.Source){
                string msg = String.Format("Empty response when searching for dictionary '{0}', audience '{1}', language '{2}' and id '{3}.", dictionary, audience, language, id);
                _logger.LogError(msg);
                throw new APIErrorException(200, msg);
            }

            return response.Source;
        }

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
        public async Task<GlossaryTermResults> GetAll(string dictionary, AudienceType audience, string language, int size, int from, string[] requestedFields)
        {
            // Dummy return for now.
            GlossaryTermResults results = new GlossaryTermResults()
            {
                Meta = new ResultsMetadata()
                {
                    TotalResults = 200,
                    From = 20
                },
                Links = new Metalink()
                {
                    Self = new System.Uri("https://www.cancer.gov")
                },
                Results = new GlossaryTerm[]
                {
                    new GlossaryTerm()
                    {
                        TermId =43966,
                        Language = "en",
                        Dictionary = "Cancer.gov",
                        Audience = AudienceType.HealthProfessional,
                        TermName = "stage II cutaneous T-cell lymphoma",
                        PrettyUrlName = "stage-ii-cutaneous-t-cell-lymphoma",
                        Pronunciation = new Pronunciation()
                        {
                            Key = "kyoo-TAY-nee-us T-sel lim-FOH-muh",
                            Audio = "https://www.cancer.gov/PublishedContent/Media/CDR/media/703959.mp3"
                        },
                        Definition = new Definition()
                        {
                            Html = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells.",
                            Text = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells."
                        }
                    },
                    new GlossaryTerm()
                    {
                        TermId =43971,
                        Language = "en",
                        Dictionary = "Cancer.gov",
                        Audience = AudienceType.Patient,
                        TermName = "bcl-2 antisense oligodeoxynucleotide G3139",
                        PrettyUrlName = "bcl-2-antisense-oligodeoxynucleotide-g3139",
                        Pronunciation = new Pronunciation()
                        {
                            Key = "AN-tee-sents AH-lih-goh-dee-OK-see-NOO-klee-oh-tide",
                            Audio = "https://www.cancer.gov/PublishedContent/Media/CDR/media/703968mp3"
                        },
                        Definition = new Definition()
                        {
                            Html = "A substance being studied in the treatment of cancer. It may kill cancer cells by blocking the production of a protein that makes cancer cells live longer and by making them more sensitive to anticancer drugs. It is a type of antisense oligodeoxyribonucleotide. Also called augmerosen, Genasense, and oblimersen sodium.",
                            Text = "A substance being studied in the treatment of cancer. It may kill cancer cells by blocking the production of a protein that makes cancer cells live longer and by making them more sensitive to anticancer drugs. It is a type of antisense oligodeoxyribonucleotide. Also called augmerosen, Genasense, and oblimersen sodium."
                        }
                    }
                }
            };

            return results;
        }

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
        public async Task<List<GlossaryTerm>> Search(string dictionary, AudienceType audience, string language, string query,string matchType, int size, int from, string[] requestedFields)
        {
            // Temporary Solution till we have Elastic Search
            List<GlossaryTerm> glossaryTermList = new List<GlossaryTerm>();
            glossaryTermList.Add(GenerateSampleTerm(requestedFields));
            glossaryTermList.Add(GenerateSampleTerm(requestedFields));

            return glossaryTermList;
        }


        /// <summary>
        /// Search for Terms based on the character passed.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="expandCharacter">The character to search the query</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when requestedFields is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when requestedFields contains one or more null values</exception>
        /// </summary>
        public async Task<GlossaryTermResults> Expand(string dictionary, AudienceType audience, string language, string expandCharacter, int size, int from, string[] requestedFields)
        {
            // Elasticsearch knows how to figure out what the ElasticSearch name is for
            // a given field when given a PropertyInfo.
            Field[] requestedESFields = convertRequestedFieldsToProperties(requestedFields)
                                            .Select(pi => new Field(pi))
                                            .ToArray();

            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(new string[] { this._apiOptions.AliasName});
            Types types = Types.Type(new string[] { "terms" });
            SearchRequest request = new SearchRequest(index, types)
            {
                Query = new TermQuery {Field = "language", Value = language.ToString()} &&
                        new TermQuery {Field = "audience", Value = audience.ToString()} &&
                        new TermQuery {Field = "dictionary", Value = dictionary.ToString()} &&
                        new TermQuery {Field = "first_letter", Value = expandCharacter.ToString()}
                ,
                Sort = new List<ISort>
                {
                    new SortField { Field = "term_name" }
                },
                Size = size,
                From = from,
                Source = new SourceFilter
                {
                    Includes = requestedESFields
                }
            };

            ISearchResponse<GlossaryTerm> response = null;
            try
            {
                response = await _elasticClient.SearchAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = String.Format("Could not search dictionary '{0}', audience '{1}', language '{2}', character '{3}', size '{4}', from '{5}'.", dictionary, audience, language, expandCharacter, size, from);
                _logger.LogError(msg, ex);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = String.Format("Invalid response when searching for dictionary '{0}', audience '{1}', language '{2}', character '{3}', size '{4}', from '{5}'.", dictionary, audience, language, expandCharacter, size, from);
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occured");
            }

            GlossaryTermResults glossaryTermResults = new GlossaryTermResults();

            if (response.Total > 0)
            {
                // Build the array of glossary terms for the returned results.
                List<GlossaryTerm> termResults = new List<GlossaryTerm>();
                foreach (GlossaryTerm res in response.Documents)
                {
                    termResults.Add(res);
                }

                glossaryTermResults.Results = termResults.ToArray();

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }
            else if (response.Total == 0) {
                // Add the defualt value of empty GlossaryTerm list.
                glossaryTermResults.Results = new GlossaryTerm[] {};

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }

            return glossaryTermResults;
        }

        /// <summary>
        /// Yields a collection of PropertyInfo objects representing the named requestedFields passed into the API.
        /// </summary>
        /// <param name="requestedFields">The requested fields. An array of nulls will be treated as an empty array.</param>
        /// <returns>An array of PropertyInfo objects representing the fields. NOTE: If a field is a value type then it
        /// must be returned as part of the object so that we do not get incorrect default values. Dotnet cannot actually change
        /// the shape of our object, or at least not without major trouble. So we will force the return here.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when requestedFields is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when requestedFields contains one or more null values.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown field is requested.</exception>
        private IEnumerable<PropertyInfo> convertRequestedFieldsToProperties(string[] requestedFields)
        {
            if (requestedFields == null)
            {
                throw new ArgumentNullException("requestedFields");
            }

            if (requestedFields.Contains(null))
            {
                throw new ArgumentException("Null requested field encountered.", "requestedFields");
            }

            // Lowercase our field names.
            string[] fieldsList = requestedFields
                                    .Select(f => f.ToLower())
                                    .ToArray();

            // Now we will use reflection to get a list of the properties of the GlossaryTerm class.
            // Then we will loop through returning only those that match our list of names
            Type glossaryTermType = typeof(GlossaryTerm);
            PropertyInfo[] properties = glossaryTermType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propertyNames = properties.Select(p => p.Name.ToLower());
            var badFields = fieldsList.Except(propertyNames);

            if (badFields.Count() > 0)
            {
                throw new ArgumentOutOfRangeException("requestedFields", $"Unknown fields: {String.Join(", ", badFields)} ");
            }

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.IsValueType)
                {
                    // This should be Ints, Enums, etc.
                    // It will not be strings.
                    yield return property;
                }
                else if (fieldsList.Contains(property.Name.ToLower()))
                {
                    yield return property;
                }
            }
        }

        /// <summary>
        /// Get the total number of terms available in the version of a dictionary matching a specific audience and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <returns>The number of terms available.</returns>
        public async Task<long> GetCount(string dictionary, AudienceType audience, string language)
        {
            // Set up the CountRequest to send to elasticsearch.
            Indices index = Indices.Index(new string[] { this._apiOptions.AliasName });
            Types types = Types.Type(new string[] { "terms" });
            CountRequest request = new CountRequest(index, types)
            {
                Query = new TermQuery { Field = "language", Value = language.ToString() } &&
                        new TermQuery { Field = "audience", Value = audience.ToString() } &&
                        new TermQuery { Field = "dictionary", Value = dictionary.ToString() }
            };

            ICountResponse response = null;
            try
            {
                response = await _elasticClient.CountAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = $"Could not get a count for dictionary '{dictionary}', audience '{audience}', language '{language}'";
                _logger.LogError(msg, ex);
                throw new APIErrorException(500, msg);
            }

            if(!response.IsValid)
            {
                String msg = $"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}'";
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occured");
            }

            return response.Count;
        }

        /// <summary>
        /// This temporary method will create a GlossaryTerm
        /// object to testing purpose.
        /// </summary>
        /// <returns>The GlossaryTerm</returns>
        private GlossaryTerm GenerateSampleTerm(string[] requestedFields){
            GlossaryTerm _GlossaryTerm = new GlossaryTerm();
            Pronunciation pronunciation = new Pronunciation("Pronunciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            _GlossaryTerm.TermId = 7890L;
            _GlossaryTerm.Language = "EN";
            _GlossaryTerm.Dictionary = "Dictionary";
            _GlossaryTerm.Audience = AudienceType.Patient;
            _GlossaryTerm.TermName = "TermName";
            _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
            _GlossaryTerm.Pronunciation = pronunciation;
            _GlossaryTerm.Definition = definition;
            foreach (string field in requestedFields)
            {
                if(field.Equals("Id")){
                    _GlossaryTerm.TermId = 1234L;
                }else  if(field.Equals("Language",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Language = "EN";
                }else  if(field.Equals("Dictionary",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Dictionary = "Dictionary";
                }else  if(field.Equals("Audience",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Audience = AudienceType.Patient;
                }else  if(field.Equals("TermName",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.TermName = "TermName";
                }else  if(field.Equals("PrettyUrlName",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
                }else  if(field.Equals("Pronunciation",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Pronunciation = pronunciation;
                }else  if(field.Equals("Definition",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Definition = definition;
                }
            }

            _GlossaryTerm.RelatedResources = new IRelatedResource[] {
                new LinkResource()
                {
                    Type = RelatedResourceType.External,
                    Text = "Link to Google",
                    Url = new System.Uri("https://www.google.com")
                },
                new LinkResource()
                {
                    Type = RelatedResourceType.DrugSummary,
                    Text = "Bevacizumab",
                    Url = new System.Uri("https://www.cancer.gov/about-cancer/treatment/drugs/bevacizumab")
                },
                new LinkResource()
                {
                    Type = RelatedResourceType.Summary,
                    Text = "Lung cancer treatment",
                    Url = new System.Uri("https://www.cancer.gov/types/lung/patient/small-cell-lung-treatment-pdq")
                },
                new GlossaryResource()
                {
                    Type = RelatedResourceType.GlossaryTerm,
                    Text = "stage II cutaneous T-cell lymphoma",
                    TermId = 43966,
                    Audience = AudienceType.Patient,
                    PrettyUrlName = "stage-ii-cutaneous-t-cell-lymphoma"
                }
            };
            return _GlossaryTerm;
        }


    }
}
