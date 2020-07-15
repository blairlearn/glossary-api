using Newtonsoft.Json.Linq;

using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Base class for test data objects in mock Elasticsearch requests.
    /// </summary>
    public abstract class BaseAutosuggestRequestTestData
    {
        /// <summary>
        /// Gets the expected data object. This is what the request is supposed to look like.
        /// </summary>
        public abstract JObject ExpectedData { get; }

        /// <summary>
        /// The text the mock search suggestion should use.
        /// </summary>
        public abstract string SearchText { get; }

        /// <summary>
        /// Should this be a search for terms containg SearchText? Or beginning with?
        /// Set TRUE for contains.
        /// Set FALSE for begins with.
        /// </summary>
        public abstract MatchType MatchType { get; }

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

        /// <summary>
        /// The number of records to return.
        /// </summary>
        public abstract int Size { get; }

    }
}