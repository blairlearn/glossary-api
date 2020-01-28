using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Base class for test data objects in mock Elasticsearch responses.
    /// </summary>
    public abstract class BaseAutosuggestTestData
    {
        /// <summary>
        /// Contains the name of the JSON data file to use in
        /// </summary>
        public abstract string TestFilename { get; }

        /// <summary>
        /// Gets the expected response data object.
        /// </summary>
        public abstract Suggestion[] ExpectedData { get; }

    }
}