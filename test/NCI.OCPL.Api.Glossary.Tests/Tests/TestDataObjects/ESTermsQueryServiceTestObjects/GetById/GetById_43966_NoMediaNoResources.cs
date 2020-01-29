using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermsQueryTestData
{
    public class GetById_43966_NoMediaNoResources : BaseTermsQueryTestData
    {
        public override string DictionaryName => "cancer.gov";
        public override AudienceType Audience => AudienceType.Patient;
        public override long TermID => 43966L;

        public override string ESTermID => "43966_cancer.gov_en_patient";
        public override string Language => "en";

        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
                TermId = 43966L,
                Language = "en",
                Dictionary = "Cancer.gov",
                Audience = AudienceType.Patient,
                TermName = "stage II cutaneous T-cell lymphoma",
                FirstLetter = "s",
                PrettyUrlName = "stage-ii-cutaneous-t-cell-lymphoma",
                Definition = new Definition()
                {
                    Text = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells.",
                    Html = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells."
                },
                Pronunciation = new Pronunciation()
                {
                    Key = "(... kyoo-TAY-nee-us T-sel lim-FOH-muh)",
                    Audio = "https://nci-media-dev.cancer.gov/audio/pdq/703959.mp3"
                }
        };
    }
}