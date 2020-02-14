using System;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public abstract class SearchTermsQueryTestData
    {
        /// <summary>
        /// Gets an instance of the Expected Data object
        /// </summary>
        /// <returns></returns>
        public abstract GlossaryTermResults ExpectedData { get; }

/// <summary>
        /// Gets the type of Search test we are performing
        /// </summary>
        /// <returns></returns>
        public abstract string SearchTestType { get; }
    }
}