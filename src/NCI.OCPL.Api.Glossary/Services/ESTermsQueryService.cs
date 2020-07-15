using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// <returns>An object of GlossaryTerm</returns>
        /// </summary>
        public async Task<GlossaryTerm> GetById(string dictionary, AudienceType audience, string language, long id)
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
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}' and id '{id}.";
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = $"Invalid response when searching for dictionary  '{dictionary}', audience '{audience}', language '{language}' and id '{id}.";
                _logger.LogError(msg);
                throw new APIErrorException(500, msg);
            }

            if(null==response.Source){
                string msg = $"No match for dictionary '{dictionary}', audience '{audience}', language '{language}' and id '{id}.";
                _logger.LogDebug(msg);
                throw new APIErrorException(404, msg);
            }

            return response.Source;
        }

        /// <summary>
        /// Search for Term based on the pretty URL name passed.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="prettyUrlName">The pretty url name to search for</param>
        /// <returns>An object of GlossaryTerm</returns>
        /// </summary>
        public async Task<GlossaryTerm> GetByName(string dictionary, AudienceType audience, string language, string prettyUrlName)
        {
            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(new string[] { this._apiOptions.AliasName});
            Types types = Types.Type(new string[] { "terms" });
            SearchRequest request = new SearchRequest(index, types)
            {
                Query = new TermQuery {Field = "language", Value = language.ToString()} &&
                        new TermQuery {Field = "audience", Value = audience.ToString()} &&
                        new TermQuery {Field = "dictionary", Value = dictionary.ToString()} &&
                        new TermQuery {Field = "pretty_url_name", Value = prettyUrlName.ToString()}
                ,
                Sort = new List<ISort>
                {
                    new SortField { Field = "term_name" }
                }
            };

            ISearchResponse<GlossaryTerm> response = null;
            try
            {
                response = await _elasticClient.SearchAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'.";
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = $"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'.";
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occured");
            }

            GlossaryTerm glossaryTerm = new GlossaryTerm();

            // If there is only one term in the response, then the search by pretty URL name was successful.
            if (response.Total == 1)
            {
                glossaryTerm = response.Documents.First();
            }
            else if (response.Total == 0)
            {
                string msg = $"No match for dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'.";
                _logger.LogDebug(msg);
                throw new APIErrorException(404, msg);
            }
            else {
                string msg = $"Incorrect response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'.";
                _logger.LogError(msg);
                throw new APIErrorException(500, "Errors have occured.");
            }

            return glossaryTerm;
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
                        new TermQuery {Field = "dictionary", Value = dictionary.ToString()}
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
                String msg = $"Could not get dictionary '{dictionary}', audience '{audience}', language '{language}', size '{size}', from '{from}'.";
                _logger.LogError($"Error Fetching All from index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = $"Invalid response when getting dictionary '{dictionary}', audience '{audience}', language '{language}', size '{size}', from '{from}'.";
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
        public async Task<GlossaryTermResults> Search(string dictionary, AudienceType audience, string language, string query, MatchType matchType, int size, int from, string[] requestedFields)
        {
            // Elasticsearch knows how to figure out what the ElasticSearch name is for
            // a given field when given a PropertyInfo.
            Field[] requestedESFields = convertRequestedFieldsToProperties(requestedFields)
                                            .Select(pi => new Field(pi))
                                            .ToArray();

            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(new string[] { this._apiOptions.AliasName});
            Types types = Types.Type(new string[] { "terms" });

            // Figure out the specific term subquery based on the match type.
            QueryBase termSubquery;
            switch (matchType)
            {
                case MatchType.Begins: termSubquery = new PrefixQuery { Field = "term_name", Value = query }; break;
                case MatchType.Contains: termSubquery = new MatchQuery { Field = "term_name._contain", Query = query }; break;
                case MatchType.Exact: termSubquery = new TermQuery { Field = "term_name", Value = query }; break;
                default:
                    throw new ArgumentException($"Uknown matchType value '${matchType}'.");
            }

            SearchRequest request = new SearchRequest(index, types)
            {
                Query = new TermQuery {Field = "language", Value = language.ToString()} &&
                        new TermQuery {Field = "audience", Value = audience.ToString()} &&
                        new TermQuery {Field = "dictionary", Value = dictionary.ToString()} &&
                        termSubquery
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
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}', query '{query}', size '{size}', from '{from}'.";
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(msg, ex);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = $"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}', query '{query}', size '{size}', from '{from}'.";
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
        /// Search for Terms based on the character passed.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="expandCharacter">The character to search the query</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
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
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}', character '{expandCharacter}', size '{size}', from '{from}'.";
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(msg, ex);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                String msg = $"Invalid response when searching for '{dictionary}', audience '{audience}', language '{language}', character '{expandCharacter}', size '{size}', from '{from}'.";
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
                _logger.LogError($"Error getting count on index: '{this._apiOptions.AliasName}'.");
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

    }
}
