using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class GetByName_s_phase_fraction : GetByNameTermsQueryTestData
    {
        public override GlossaryTerm ExpectedData => new GlossaryTerm() {
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
            Pronunciation = new Pronunciation() {
                Key = "(... fayz FRAK-shun)",
                Audio = "https://nci-media-dev.cancer.gov/audio/pdq/705947.mp3"
            },
            Media = new IMedia[] {},
            RelatedResources = new IRelatedResource[] { }
        };

        public override string PrettyUrlName => "s-phase-fraction";
    }
}