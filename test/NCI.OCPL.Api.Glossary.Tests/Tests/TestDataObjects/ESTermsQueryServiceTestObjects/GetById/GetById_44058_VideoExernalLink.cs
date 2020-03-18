using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class GetById_44058_VideoExernalLink : BaseTermsQueryTestData
    {
        public override string DictionaryName => "cancer.gov";
        public override AudienceType Audience => AudienceType.Patient;
        public override long TermID => 44058L;
        public override string ESTermID => "44058_cancer.gov_en_patient";
        public override string Language => "en";

        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
            TermId = 44058L,
            Language = "en",
            Dictionary = "Cancer.gov",
            Audience = AudienceType.Patient,
            TermName = "metastatic",
            FirstLetter = "m",
            PrettyUrlName = "metastatic",
            Definition = new Definition()
            {
                Text = "Having to do\n          with metastasis, which is the spread of cancer from the\n          primary site (place where it started) to other places in\n          the body.",
                Html = "Having to do\n          with metastasis, which is the spread of cancer from the\n          primary site (place where it started) to other places in\n          the body."
            },
            Pronunciation = new Pronunciation()
            {
                Key = "(meh-tuh-STA-tik)",
                Audio = "https://nci-media-dev.cancer.gov/audio/pdq/704104.mp3"
            },
            Media = new IMedia[] {
                new Image {
                Type = MediaType.Image,
                Ref = "CDR0000764135",
                Alt = "Metastasis; drawing shows primary cancer that has spread from the colon to other parts of the body (the liver and the lung). An inset shows cancer cells spreading from the primary cancer, through the blood and lymph system, to another part of the body where a metastatic tumor has formed.",
                Caption = "Metastasis. In metastasis, cancer cells break away from where they first formed (primary cancer), travel through the blood or lymph system, and form new tumors (metastatic tumors) in other parts of the body. The metastatic tumor is the same type of cancer as the primary tumor. ",
                ImageSources = new ImageSource[] {
                    new ImageSource {
                        Size = "original",
                        Src = new Uri("https://nci-media-dev.cancer.gov/images/pdq/CDR764135.jpg")
                    },
                    new ImageSource {
                        Size = "571",
                        Src = new Uri("https://nci-media-dev.cancer.gov/images/pdq/CDR764135-571.jpg")
                    },
                    new ImageSource {
                        Size = "750",
                        Src = new Uri("https://nci-media-dev.cancer.gov/images/pdq/CDR764135-750.jpg")
                    }
                }
                //Template = "image-center",
                },
                new Video {
                    Type = MediaType.Video,
                    Ref = "CDR0000787719",
                    Hosting = HostingTypes.youtube,
                    UniqueId = "fQwar_-QdiQ",
                    Caption = "Many cancer deaths are caused when cancer moves from the original tumor and spreads to other tissues and organs. This is called metastatic cancer. This animation shows how cancer cells travel from the place in the body where they first formed to other parts of the body.",
                    Template = "Video75NoTitle",
                    Title = "Metastasis: How Cancer Spreads"
                }
            },
            OtherLanguages = new TermOtherLanguage[] {
                new TermOtherLanguage {
                    Language = "es",
                    TermName = "metastásico",
                    PrettyUrlName = "metastasico"
                }
            },
            RelatedResources = new IRelatedResource[] {
                new LinkResource {
                    Text = "Metastatic Cancer",
                    Url = new Uri("https://www.cancer.gov/types/metastatic-cancer"),
                    Type = RelatedResourceType.External
                }
            }
        };
    }
}
