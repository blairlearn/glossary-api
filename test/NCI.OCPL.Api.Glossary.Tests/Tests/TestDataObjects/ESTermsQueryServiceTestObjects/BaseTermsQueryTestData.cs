using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public abstract class BaseTermsQueryTestData
    {
        /// <summary>
        /// Gets the full Elasticsearch Term ID for this term.
        /// This will be used as the file name.
        /// </summary>
        /// <returns></returns>
        public abstract string ESTermID { get; }

        /// <summary>
        /// Gets an instance of the Expected Data object
        /// </summary>
        /// <returns></returns>
        public abstract GlossaryTerm ExpectedData { get; }

        /// <summary>
        /// The name of the dictionary for the term request
        /// </summary>
        /// <value></value>
        public abstract string DictionaryName { get; }

        /// <summary>
        /// The language of the term request
        /// </summary>
        /// <value></value>
        public abstract string Language { get; }

        /// <summary>
        /// The ID for the term request
        /// </summary>
        /// <value></value>
        public abstract long TermID { get; }

        /// <summary>
        /// The audience type for the term request
        /// </summary>
        /// <value></value>
        public abstract AudienceType Audience { get; }
    }
}