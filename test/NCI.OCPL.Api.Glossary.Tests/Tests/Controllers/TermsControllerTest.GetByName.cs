using System;

using Moq;
using Xunit;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Glossary.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using NCI.OCPL.Api.Common.Testing;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public partial class TermsControllerTests
    {

        [Fact]
        public async void GetByName_ErrorMessage_Dictionary()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("", AudienceType.Patient, "en", "s-1")
            );
            Assert.Equal("You must supply a valid dictionary, audience, and language.", exception.Message);
        }
        [Fact]
        public async void GetByName_ErrorMessage_EmptyLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", AudienceType.Patient, "", "s-1")
            );
            Assert.Equal("You must supply a valid dictionary, audience, and language.", exception.Message);
        }

        [Fact]
        public async void GetByName_ErrorMessage_AudienceType(){
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", (AudienceType)(-18), "en", "s-1")
            );
            Assert.Equal("You must supply a valid dictionary, audience, and language.", exception.Message);
        }

        [Fact]
        public async void GetByName_ErrorMessage_InvalidLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", AudienceType.Patient, "chicken", "s-1")
            );
            Assert.Equal("Unsupported Language. Please try either 'en' or 'es'", exception.Message);
        }

        /// <summary>
        /// Verify that GetByName endpoint makes the correct call to the query service,
        /// and returns the correct response for the specified parameters.
        /// </summary>
        [Fact]
        public async void GetByNameTerms()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);

            GlossaryTerm glossaryTerm = new GlossaryTerm() {
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
            };

            termsQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            GlossaryTerm term = await controller.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-phase-fraction");
            string actualJsonValue = JsonConvert.SerializeObject(term);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetByName.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-phase-fraction"),
                Times.Once
            );

            Assert.Equal(glossaryTerm, term, new GlossaryTermComparer());
            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

    }
}