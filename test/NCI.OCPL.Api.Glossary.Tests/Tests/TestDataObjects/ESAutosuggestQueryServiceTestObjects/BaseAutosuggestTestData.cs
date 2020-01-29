using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    public abstract class BaseAutosuggestTestData
    {
        /// <summary>
        /// Gets the name of the JSON data file corresponding
        /// to this instance.
        /// </summary>
        /// <value></value>
        public abstract string filename { get; }

        /// <summary>
        /// Gets the Expected Data object
        /// </summary>
        /// <returns></returns>
        public abstract Suggestion[] ExpectedData { get; }

        /// <summary>
        /// Set TRUE for "Begins with"
        /// Set FALSE for "Contains"
        /// </summary>
        /// <value></value>
        public abstract bool BeginsWith { get; }

        /// <summary>
        /// The name of the dictionary for the suggestion request
        /// </summary>
        /// <value></value>
        public abstract string DictionaryName { get; }

        /// <summary>
        /// The language of the suggestion request
        /// </summary>
        /// <value></value>
        public abstract string Language { get; }

        /// <summary>
        /// The audience type for the suggestion request
        /// </summary>
        /// <value></value>
        public abstract AudienceType Audience { get; }

    }
}