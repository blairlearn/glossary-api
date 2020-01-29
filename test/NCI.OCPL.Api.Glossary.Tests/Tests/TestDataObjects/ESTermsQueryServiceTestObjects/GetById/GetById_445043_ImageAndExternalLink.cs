using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermsQueryTestData
{
    public class GetById_445043_ImageAndExternalLink : BaseTermsQueryTestData
    {
        public override string DictionaryName => "cancer.gov";
        public override AudienceType Audience => AudienceType.Patient;
        public override long TermID => 445043L;
        public override string ESTermID => "445043_cancer.gov_en_patient";
        public override string Language => "en";

        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
            TermId = 445043L,
            Language = "en",
            Dictionary = "Cancer.gov",
            Audience = AudienceType.Patient,
            TermName = "lung cancer",
            FirstLetter = "l",
            PrettyUrlName = "lung-cancer",
            Definition = new Definition()
            {
                Text = "Cancer that forms in tissues of the lung, usually in the cells lining air passages. The two main types are small cell lung cancer and non-small cell lung cancer. These types are diagnosed based on how the cells look under a microscope.",
                Html = "Cancer that forms in tissues of the lung, usually in the cells lining air passages. The two main types are small cell lung cancer and non-small cell lung cancer. These types are diagnosed based on how the cells look under a microscope."
            },
            Pronunciation = new Pronunciation()
            {
                Key = "(lung KAN-ser)",
                Audio = "https://nci-media-dev.cancer.gov/audio/pdq/714622.mp3"
            },
            Media = new IMedia[] {
                new Image {
                    Type = MediaType.Image,
                    Ref = "CDR0000466533",
                    Alt = "Respiratory anatomy; drawing shows right lung with upper, middle, and lower lobes; left lung with upper and lower lobes; and the trachea, bronchi, lymph nodes, and diaphragm. Inset shows bronchioles, alveoli, artery, and vein.  \n\n",
                    Caption = "Anatomy of the respiratory system, showing the trachea and both lungs and their lobes and airways. Lymph nodes and the diaphragm are also shown. Oxygen is inhaled into the lungs and passes through the thin membranes of the alveoli and into the bloodstream (see inset).",
                    ImageSources = new ImageSource[] {
                        new ImageSource {
                            Size = "original",
                            Src = new Uri("https://nci-media-dev.cancer.gov/images/pdq/CDR466533.jpg")
                        },
                        new ImageSource {
                            Size = "571",
                            Src = new Uri("https://nci-media-dev.cancer.gov/images/pdq/CDR466533-571.jpg")
                        },
                        new ImageSource {
                            Size = "750",
                            Src = new Uri("https://nci-media-dev.cancer.gov/images/pdq/CDR466533-750.jpg")
                        }
                    }
                //"template": "image-center"
                }
            },
            RelatedResources = new IRelatedResource[] {
                new LinkResource {
                    Text = "Lung Cancer",
                    Url = new Uri("https://www.cancer.gov/types/lung"),
                    Type = RelatedResourceType.External
                }
            }
        };
    }
}
