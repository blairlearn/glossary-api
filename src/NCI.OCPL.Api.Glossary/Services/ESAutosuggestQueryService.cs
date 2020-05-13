using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary.Models;
using System;

namespace NCI.OCPL.Api.Glossary.Services
{
    /// <summary>
    /// Elasticsearch implementation of the service for retrieveing suggestions for
    /// GlossaryTerm objects.
    /// </summary>
    public class ESAutosuggestQueryService : IAutosuggestQueryService
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
        private readonly ILogger<ESAutosuggestQueryService> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ESAutosuggestQueryService(IElasticClient client,
            IOptions<GlossaryAPIOptions> apiOptionsAccessor,
            ILogger<ESAutosuggestQueryService> logger)
        {
            _elasticClient = client;
            _apiOptions = apiOptionsAccessor.Value;
            _logger = logger;
        }

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// </summary>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="searchText">The text to search for.</param>
        /// <param name="matchType">Set to true to allow search to find terms which contain the query string instead of explicitly starting with it.</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <returns>An array of Suggestion objects</returns>
        public async Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string searchText, MatchType matchType, int size)
        {
            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(new string[] { this._apiOptions.AliasName });
            Types types = Types.Type(new string[] { "terms" });

            ISearchResponse<Suggestion> response = null;

            try
            {
                SearchRequest request;
                switch (matchType)
                {
                    default:
                    case MatchType.Begins:
                        request = BuildBeginRequest(index, types, dictionary, language, audience, searchText, size);
                        break;
                    case MatchType.Contains:
                        request = BuildContainsRequest(index, types, dictionary, language, audience, searchText, size);
                        break;
                }

                response = await _elasticClient.SearchAsync<Suggestion>(request);
            }
            catch (Exception ex)
            {
                string msg = $"Could not search dictionary '{dictionary}', audience '{audience}', and language '{language}'.";
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValid)
            {
                _logger.LogError($"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}', query '{searchText}', contains '{matchType}', size '{size}'.");
                throw new APIErrorException(500, "errors occured");
            }

            List<Suggestion> retVal = new List<Suggestion>(response.Documents);

            return retVal.ToArray();
        }

        /// <summary>
        /// Builds the SearchRequest for terms beginning with the search text.
        /// </summary>
        /// <param name="index">The index which will be searched against.</param>
        /// <param name="types">The list of document types to search.</param>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The text to search for.</param>
        /// <param name="size">The number of records to retrieve.</param>
        private SearchRequest BuildBeginRequest(Indices index, Types types, string dictionary, string language, AudienceType audience, string query, int size)
        {
            /*
             * Create a query similar to the following.  The bool subquery is written with an overloaded version
             * of operator && which supplies the `{"bool" :  {"must": [` portion for you.
             * This is somewhat explained here: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/bool-queries.html.
             *
             * curl -XPOST http://SERVER_NAME/glossaryv1/terms/_search -H 'Content-Type: application/x-ndjson'   -d '{
             *   "query": {
             *     "bool" : {
             *       "must" :
             *         [{"term" : { "language" : "es" }},
             *         {"term": { "audience": "Patient"}},
             *         {"term": { "dictionary": "Cancer.gov"}},
             *         {"prefix" : {"term_name" : "cutáneo"}}
             *       ]
             *     }
             *   }
             * ,"sort": ["term_name"]
             * , "_source": ["term_id", "term_name"]
             * , "size": 10
             * }'
             */
            SearchRequest request = new SearchRequest(index, types)
            {
                Query = new TermQuery {Field = "language", Value = language } &&
                        new TermQuery {Field = "audience", Value = audience.ToString() } &&
                        new TermQuery {Field = "dictionary", Value = dictionary } &&
                        new PrefixQuery {Field = "term_name", Value = query },
                Sort = new List<ISort>
                {
                    new SortField { Field = "term_name" }
                },
                Source = new SourceFilter
                {
                    Includes = new string[]{"term_id", "term_name"}
                },
                Size = size
            };

            return request;
        }

        /// <summary>
        /// Builds the SearchRequest for terms containing with the search text.
        /// </summary>
        /// <param name="index">The index which will be searched against.</param>
        /// <param name="types">The list of document types to search.</param>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The text to search for.</param>
        /// <param name="size">The number of records to retrieve.</param>
        private SearchRequest BuildContainsRequest(Indices index, Types types, string dictionary, string language, AudienceType audience, string query, int size)
        {
            /*
             * Create a query similar to the following.  The bool subquery is written with an overloaded version
             * of operator && which supplies the `{"bool" :  {"must": [` portion for you.
             * This is somewhat explained here: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/bool-queries.html.
             *
             * curl -XPOST http://SERVER_NAME/glossaryv1/terms/_search -H 'Content-Type: application/x-ndjson'   -d '{
             *   "query": {
             *     "bool" : {
             *       "must" :
             *         [{"term" : { "language" : "es" }},
             *         {"term" : { "dictionary" : "Cancer.gov" }},
             *         {"term": { "audience": "Patient"}},
             *         {"match_phrase":{"term_name._autocomplete":"cutáneo"}}
             *         ],
             *          "must_not" :  {"prefix" : {"term_name" : "cutáneo"}}
             *       }
             *     }
             * ,"sort": ["term_name"]
             * , "_source": ["term_id", "term_name"]
             * , "from": 0
             * , "size": 10
             * }'
             */
            SearchRequest request = new SearchRequest(index, types)
            {
                Query = new TermQuery {Field = "language", Value = language.ToString() } &&
                        new TermQuery {Field = "audience", Value = audience.ToString() } &&
                        new TermQuery {Field = "dictionary", Value = dictionary.ToString() } &&
                        new MatchPhraseQuery {Field = "term_name._autocomplete", Query = query.ToString() } &&
                        !new PrefixQuery {Field = "term_name", Value = query.ToString() },
                Sort = new List<ISort>
                {
                    new SortField { Field = "term_name" }
                },
                Source = new SourceFilter
                {
                    Includes = new string[] { "term_id", "term_name" }
                },
                Size = size
            };

            return request;
        }

    }
}