using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class Expand_NoResults : ExpandTermsQueryTestData
    {
        public override GlossaryTermResults ExpectedData => new GlossaryTermResults() {
            Results = new GlossaryTerm[] {},
            Meta = new ResultsMetadata() {
                TotalResults = 0,
                From = 0
            }
        };

        public override string ExpandTestType => "noresults";
    }
}