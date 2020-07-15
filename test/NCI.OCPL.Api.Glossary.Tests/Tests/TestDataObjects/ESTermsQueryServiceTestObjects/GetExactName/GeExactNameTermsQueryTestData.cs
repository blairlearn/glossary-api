using System;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public abstract class GeExactNameTermsQueryTestData
    {
        /// <summary>
        /// Gets an instance of the Expected Data object
        /// </summary>
        /// <returns></returns>
        public abstract GlossaryTerm ExpectedData { get; }

        /// <summary>
        /// Gets the term name for the object
        /// </summary>
        /// <returns></returns>
        public abstract string TermName { get; }
    }
}