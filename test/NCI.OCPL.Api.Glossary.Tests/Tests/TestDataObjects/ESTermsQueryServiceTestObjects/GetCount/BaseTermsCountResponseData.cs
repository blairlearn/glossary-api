using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public abstract class BaseTermsCountResponseData
    {
        /// <summary>
        /// Contains the name of the JSON data file to use in
        /// </summary>
        public abstract string TestFilename { get; }

        /// <summary>
        /// The expected result count.
        /// </summary>
        public abstract long ExpectedCount { get; }
    }
}