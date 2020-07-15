using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class GetExactName_s_1 : GeExactNameTermsQueryTestData
    {
        public override GlossaryTerm ExpectedData => new GlossaryTerm() {
            TermId = 46716,
            Language = "en",
            Dictionary = "Cancer.gov",
            Audience = AudienceType.Patient,
            TermName = "S-1",
            FirstLetter = "s",
            PrettyUrlName = "s-1",
            Definition = new Definition()
            {
                Text = "A drug that is being studied for its ability to enhance the effectiveness of fluorouracil and prevent gastrointestinal side effects caused by fluorouracil. It belongs to the family of drugs called antimetabolites.",
                Html = "A drug that is being studied for its ability to enhance the effectiveness of fluorouracil and prevent gastrointestinal side effects caused by fluorouracil. It belongs to the family of drugs called antimetabolites."
            },
            Pronunciation = null,
            Media = new IMedia[] {},
            RelatedResources = new IRelatedResource[] { }
        };

        public override string TermName => "s-1";
    }
}