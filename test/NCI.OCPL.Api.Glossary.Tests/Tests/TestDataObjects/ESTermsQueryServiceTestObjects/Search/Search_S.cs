using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class Search_S : SearchTermsQueryTestData
    {
        public override GlossaryTermResults ExpectedData => new GlossaryTermResults() {
            Results = new GlossaryTerm[] {
                new GlossaryTerm()
                {
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
                },
                new GlossaryTerm()
                {
                    TermId = 44771,
                    Language = "en",
                    Dictionary = "Cancer.gov",
                    Audience = AudienceType.Patient,
                    TermName = "S-phase fraction",
                    FirstLetter = "s",
                    PrettyUrlName = "s-phase-fraction",
                    Definition = new Definition()
                    {
                        Text = "A measure of the percentage of cells in a tumor that are in the phase of the cell cycle during which DNA is synthesized. The S-phase fraction may be used with the proliferative index to give a more complete understanding of how fast a tumor is growing.",
                        Html = "A measure of the percentage of cells in a tumor that are in the phase of the cell cycle during which DNA is synthesized. The S-phase fraction may be used with the proliferative index to give a more complete understanding of how fast a tumor is growing."
                    },
                    Pronunciation = new Pronunciation()
                    {
                        Key = "(... fayz FRAK-shun)",
                        Audio = "https://nci-media-dev.cancer.gov/audio/pdq/705947.mp3"
                    },
                    Media = new IMedia[] {},
                    RelatedResources = new IRelatedResource[] { }
                },
                new GlossaryTerm()
                {
                    TermId = 572148,
                    Language = "en",
                    Dictionary = "Cancer.gov",
                    Audience = AudienceType.Patient,
                    TermName = "S100 calcium binding protein A8",
                    FirstLetter = "s",
                    PrettyUrlName = "s100-calcium-binding-protein-a8",
                    Definition = new Definition()
                    {
                        Text = "A protein that is made by many different types of cells and is involved in processes that take place both inside and outside of the cell. It is made in larger amounts in inflammatory diseases such as rheumatoid arthritis, and in some types of cancer. It is being studied as a biomarker for breast cancer. Also called calgranulin A.",
                        Html = "A protein that is made by many different types of cells and is involved in processes that take place both inside and outside of the cell. It is made in larger amounts in inflammatory diseases such as rheumatoid arthritis, and in some types of cancer. It is being studied as a biomarker for breast cancer. Also called calgranulin A."
                    },
                    Pronunciation = new Pronunciation()
                    {
                        Key = "(… KAL-see-um … PROH-teen …)",
                        Audio = "https://nci-media-dev.cancer.gov/audio/pdq/720720.mp3"
                    },
                    Media = new IMedia[] {},
                    RelatedResources = new IRelatedResource[] { }
                },
                new GlossaryTerm()
                {
                    TermId = 572151,
                    Language = "en",
                    Dictionary = "Cancer.gov",
                    Audience = AudienceType.Patient,
                    TermName = "S100 calcium binding protein A9",
                    FirstLetter = "s",
                    PrettyUrlName = "s100-calcium-binding-protein-a9",
                    Definition = new Definition()
                    {
                        Text = "A protein that is made by many different types of cells and is involved in processes that take place both inside and outside of the cell. It is made in larger amounts in inflammatory diseases such as rheumatoid arthritis, and in some types of cancer. It is being studied as a biomarker for breast cancer. Also called calgranulin B.",
                        Html = "A protein that is made by many different types of cells and is involved in processes that take place both inside and outside of the cell. It is made in larger amounts in inflammatory diseases such as rheumatoid arthritis, and in some types of cancer. It is being studied as a biomarker for breast cancer. Also called calgranulin B."
                    },
                    Pronunciation = new Pronunciation()
                    {
                        Key = "(… KAL-see-um … PROH-teen …)",
                        Audio = "https://nci-media-dev.cancer.gov/audio/pdq/720722.mp3"
                    },
                    Media = new IMedia[] {},
                    RelatedResources = new IRelatedResource[] { }
                },
                new GlossaryTerm()
                {
                    TermId = 651217,
                    Language = "en",
                    Dictionary = "Cancer.gov",
                    Audience = AudienceType.Patient,
                    TermName = "SAB",
                    FirstLetter = "s",
                    PrettyUrlName = "sab",
                    Definition = new Definition()
                    {
                        Text = "A temporary loss of feeling in the abdomen and/or the lower part of the body. Special drugs called anesthetics are injected into the fluid in the lower part of the spinal column to cause the loss of feeling. The patient stays awake during the procedure.  It is a type of regional anesthesia. Also called spinal anesthesia, spinal block,  and subarachnoid block.",
                        Html = "A temporary loss of feeling in the abdomen and/or the lower part of the body. Special drugs called anesthetics are injected into the fluid in the lower part of the spinal column to cause the loss of feeling. The patient stays awake during the procedure.  It is a type of regional anesthesia. Also called spinal anesthesia, spinal block,  and subarachnoid block."
                    },
                    Pronunciation = null,
                    Media = new IMedia[] {},
                    RelatedResources = new IRelatedResource[] { }
                }
            },
            Meta = new ResultsMetadata() {
                TotalResults = 854,
                From = 0
            },
            Links = null
        };

        public override string SearchTestType => "results";
    }
}