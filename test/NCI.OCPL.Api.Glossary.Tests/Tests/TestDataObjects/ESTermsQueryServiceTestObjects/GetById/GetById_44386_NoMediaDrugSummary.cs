using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermsQueryTestData
{
    public class GetById_44386_NoMediaDrugSummary : BaseTermsQueryTestData
    {
        public override string DictionaryName => "cancer.gov";
        public override AudienceType Audience => AudienceType.Patient;
        public override long TermID => 44386L;

        public override string ESTermID => "44386_cancer.gov_en_patient";
        public override string Language => "en";

        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
            TermId = 44386L,
            Language = "en",
            Dictionary = "Cancer.gov",
            Audience = AudienceType.Patient,
            TermName = "HPV",
            FirstLetter = "h",
            PrettyUrlName = "hpv",
            Definition = new Definition()
            {
                Text = "A type of virus that can cause abnormal tissue growth (for example, warts) and other changes to cells. Infection for a long time with certain types of HPV can cause cervical cancer. HPV may also play a role in some other types of cancer, such as anal, vaginal, vulvar, penile, and oropharyngeal cancers.  Also called human papillomavirus.",
                Html = "A type of virus that can cause abnormal tissue growth (for example, warts) and other changes to cells. Infection for a long time with certain types of HPV can cause cervical cancer. HPV may also play a role in some other types of cancer, such as anal, vaginal, vulvar, penile, and oropharyngeal cancers.  Also called human papillomavirus."
            },
            RelatedResources = new IRelatedResource[] {
                new LinkResource {
                    Type = RelatedResourceType.External,
                    Text = "HPV and Cancer",
                    Url = new Uri("https://www.cancer.gov/about-cancer/causes-prevention/risk/infectious-agents/hpv-and-cancer"),
                },
                new LinkResource {
                    Type = RelatedResourceType.External,
                    Text = "Human Papillomavirus (HPV) Vaccines",
                    Url = new Uri("https://www.cancer.gov/about-cancer/causes-prevention/risk/infectious-agents/hpv-vaccine-fact-sheet"),
                },
                new LinkResource {
                    Type = RelatedResourceType.DrugSummary,
                    Text = "Recombinant Human Papillomavirus (HPV) Bivalent Vaccine",
                    Url = new Uri("https://www.cancer.gov/about-cancer/treatment/drugs/recombinant-HPV-bivalent-vaccine"),
                },
                new LinkResource {
                    Type = RelatedResourceType.DrugSummary,
                    Text = "Recombinant Human Papillomavirus (HPV) Quadrivalent Vaccine",
                    Url = new Uri("https://www.cancer.gov/about-cancer/treatment/drugs/recombinant-HPV-quadrivalent-vaccine"),
                }
            }
        };
    }
}
