using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Glossary.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.Glossary.Services
{
    /// <summary>
    /// Concrete Implementation class for a Term Query service.
    /// </summary>
    public class ESTermQueryService : ITermQueryService
    {
        private IElasticClient _elasticClient;

        /// <summary>
        /// The API options.
        /// </summary>
        protected readonly GlossaryAPIOptions _apiOptions;

        /// <summary>
        /// A logger to use for logging
        /// </summary>
        private readonly ILogger<ESTermQueryService> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ESTermQueryService(IElasticClient client, IOptions<GlossaryAPIOptions> apiOptionsAccessor,
            ILogger<ESTermQueryService> logger)
        {
            _elasticClient = client;
            _apiOptions = apiOptionsAccessor.Value;
            _logger = logger;
        }

        /// <summary>
        /// Get Term deatils based on the input values
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
                string idValue = id + "_" + dictionary + "_" + language + "_" + audience.ToString().ToLower();
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
    }
}