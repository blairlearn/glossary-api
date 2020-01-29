using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermsQueryTestData
{
    public class GetById_339337_HealthProfessional : BaseTermsQueryTestData
    {
        public override string DictionaryName => "genetics";
        public override AudienceType Audience => AudienceType.HealthProfessional;
        public override long TermID => 339337L;

        public override string ESTermID => "339337_genetics_en_healthprofessional";
        public override string Language => "en";

        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
            TermId = 339337L,
            Language = "en",
            Dictionary = "Genetics",
            Audience = AudienceType.HealthProfessional,
            TermName = "allele",
            FirstLetter = "a",
            PrettyUrlName = "allele",
            Definition = new Definition()
            {
                Text = "One of two or more DNA sequences occurring at a particular gene locus. Typically one allele (“normal” DNA sequence) is common, and other alleles (mutations) are rare.",
                Html = "One of two or more DNA sequences occurring at a particular gene locus. Typically one allele (“normal” DNA sequence) is common, and other alleles (mutations) are rare."
            },
            Pronunciation = new Pronunciation()
            {
                Key = "(uh-LEEL)",
                Audio = "https://nci-media-dev.cancer.gov/audio/pdq/736960.mp3"
            },
            RelatedResources = new IRelatedResource[] {
                new LinkResource {
                    Text = "Allele",
                    Url = new Uri("https://www.genome.gov/glossary/index.cfm?id=4"),
                    Type = RelatedResourceType.External
                }
            }
        };
    }
}