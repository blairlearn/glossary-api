using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class Search_NoResults : SearchTermsQueryTestData
    {
        public override GlossaryTermResults ExpectedData => new GlossaryTermResults() {
            Results = new GlossaryTerm[] {},
            Meta = new ResultsMetadata() {
                TotalResults = 0,
                From = 0
            }
        };

        public override string SearchTestType => "noresults";
    }
}