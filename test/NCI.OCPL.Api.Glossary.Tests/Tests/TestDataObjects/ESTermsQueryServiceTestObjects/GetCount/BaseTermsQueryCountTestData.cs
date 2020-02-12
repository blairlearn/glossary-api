using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public abstract class BaseTermsQueryCountTestData
    {
        /// <summary>
        /// Gets the expected data object. This is what the request is supposed to look like.
        /// </summary>
        public abstract JObject ExpectedData { get; }

        /// <summary>
        /// The name of the dictionary for the suggestion request
        /// </summary>
        public abstract string DictionaryName { get; }

        /// <summary>
        /// The language of the suggestion request
        /// </summary>
        public abstract string Language { get; }

        /// <summary>
        /// The audience type for the suggestion request
        /// </summary>
        public abstract AudienceType Audience { get; }
    }
}