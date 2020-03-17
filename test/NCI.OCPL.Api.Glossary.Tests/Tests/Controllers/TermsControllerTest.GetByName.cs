using System;

using Moq;
using Xunit;

using Microsoft.Extensions.Logging.Testing;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Glossary.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("", AudienceType.Patient, "en", "s-1")
            );
            Assert.Equal("You must supply a valid dictionary, audience, and language.", exception.Message);
        }
        [Fact]
        public async void GetByName_ErrorMessage_EmptyLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", AudienceType.Patient, "", "s-1")
            );
            Assert.Equal("You must supply a valid dictionary, audience, and language.", exception.Message);
        }

        [Fact]
        public async void GetByName_ErrorMessage_AudienceType(){
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termsQueryService.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", (AudienceType)(-18), "en", "s-1")
            );
            Assert.Equal("You must supply a valid dictionary, audience, and language.", exception.Message);
        }

        [Fact]
        public async void GetByName_ErrorMessage_InvalidLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", AudienceType.Patient, "chicken", "s-1")
            );
            Assert.Equal("Unsupported Language. Please try either 'en' or 'es'", exception.Message);
        }

        [Fact]
        public async void GetByName_ErrorMessage_MissingPrettyUrl()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", AudienceType.Patient, "en", null)
            );
            Assert.Equal("You must specify the prettyUrlName parameter.", exception.Message);

            exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetByName("Cancer.gov", AudienceType.Patient, "en", "")
            );
            Assert.Equal("You must specify the prettyUrlName parameter.", exception.Message);
        }

        /// <summary>
        /// Verify that GetByName endpoint makes the correct call to the query service,
        /// and returns the correct response for the specified parameters.
        /// </summary>
        [Fact]
        public async void GetByNameTerms()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termsQueryService.Object);

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
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(term));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetByName.json")));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-phase-fraction"),
                Times.Once
            );

            Assert.Equal(glossaryTerm, term, new GlossaryTermComparer());
            Assert.Equal(expected, actual, new JTokenEqualityComparer());
        }

        // This test is for the HealthProfessional fallback logic for GetByName.
        // The expected returns from the terms query service for the calls expected to be made are set up.
        // The call order of the terms query service calls are set up.
        // It performs the call to the controller with the correct params.
        // It verifies the call order of the terms query service calls.
        // It verifies that the expected and actual glossaryTerm objects are the same.
        // It verifies that the query service was called the expected number of times with the expected params.
        [Fact]
        public async void GetByName_WithFallback_GeneticsHP()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            GlossaryTerm glossaryTerm = new GlossaryTerm
            {
                TermId = 556486,
                Language = "en",
                Dictionary = "Genetics",
                Audience = AudienceType.HealthProfessional,
                TermName = "deleterious mutation",
                FirstLetter = "d",
                PrettyUrlName = "deleterious-mutation",
                Pronunciation = new Pronunciation() {
                        Key = "(DEH-leh-TEER-ee-us myoo-TAY-shun)",
                        Audio = "https://nci-media-dev.cancer.gov/pdq/media/audio/736913.mp3"
                    },
                Definition = new Definition() {
                        Html = "A genetic alteration that increases an individual’s susceptibility or predisposition to a certain disease or disorder. When such a variant (or mutation) is inherited, development of symptoms is more likely, but not certain.  Also called disease-causing mutation, pathogenic variant, predisposing mutation,  and susceptibility gene mutation.",
                        Text = "A genetic alteration that increases an individual’s susceptibility or predisposition to a certain disease or disorder. When such a variant (or mutation) is inherited, development of symptoms is more likely, but not certain.  Also called disease-causing mutation, pathogenic variant, predisposing mutation,  and susceptibility gene mutation."
                    },
                RelatedResources = new IRelatedResource[] {},
                Media = new IMedia[] {}
            };

            int callOrder = 0;

            // Cancer.gov and HealthProfessional would be the first call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "Cancer.gov"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "deleterious-mutation")
                )
            )
            .Callback(() => Assert.Equal(1, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Cancer.gov', audience 'HealthProfessional', language 'en', pretty URL name 'deleterious-mutation'."));

            // Cancer.gov and Patient would be the second call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "Cancer.gov"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "deleterious-mutation")
                )
            )
            .Callback(() => Assert.Equal(2, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Cancer.gov', audience 'Patient', language 'en', pretty URL name 'deleterious-mutation'."));

            // NotSet and HealthProfessional would be the third call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "deleterious-mutation")
                )
            )
            .Callback(() => Assert.Equal(3, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'HealthProfessional', language 'en', pretty URL name 'deleterious-mutation'."));

            // NotSet and Patient would be the fourth call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "deleterious-mutation")
                )
            )
            .Callback(() => Assert.Equal(4, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'Patient', language 'en', pretty URL name 'deleterious-mutation'."));

            // Genetics and HealthProfessional would be the last call to the terms query service, returning the term.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "Genetics"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "deleterious-mutation")
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetByName("Cancer.gov", AudienceType.HealthProfessional, "en", "deleterious-mutation", true);

            // Verify that the expected and actual Term are the same.
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(gsTerm));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetWithFallback_GeneticsHP.json")));
            Assert.Equal(expected, actual, new JTokenEqualityComparer());

            // Verify that the service layer is called correctly with the fallback logic:
            // 1) Cancer.gov, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetByName("Cancer.gov", AudienceType.HealthProfessional, "en", "deleterious-mutation"),
                Times.Once
            );
            // 2) Cancer.gov, Patient
            termQueryService.Verify(
                svc => svc.GetByName("Cancer.gov", AudienceType.Patient, "en", "deleterious-mutation"),
                Times.Once
            );
            // 3) Empty dictionary, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetByName("NotSet", AudienceType.HealthProfessional, "en", "deleterious-mutation"),
                Times.Once
            );
            // 4) Empty dictionary, Patient
            termQueryService.Verify(
                svc => svc.GetByName("NotSet", AudienceType.Patient, "en", "deleterious-mutation"),
                Times.Once
            );
            // 5) Genetics, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetByName("Genetics", AudienceType.HealthProfessional, "en", "deleterious-mutation"),
                Times.Once
            );
        }

        // This test is for the Patient fallback logic for GetByName.
        // The expected returns from the terms query service for the calls expected to be made are set up.
        // The call order of the terms query service calls are set up.
        // It performs the call to the controller with the correct params.
        // It verifies the call order of the terms query service calls.
        // It verifies that the expected and actual glossaryTerm objects are the same.
        // It verifies that the query service was called the expected number of times with the expected params.
        [Fact]
        public async void GetByName_WithFallback_TermsPatient()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
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

            int callOrder = 0;

            // NotSet and Patient would be the first call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "s-phase-fraction")
                )
            )
            .Callback(() => Assert.Equal(1, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'Patient', language 'en', pretty URL name 's-phase-fraction'."));

            // NotSet and HealthProfessional would be the second call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "s-phase-fraction")
                )
            )
            .Callback(() => Assert.Equal(2, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'HealthProfessional', language 'en', pretty URL name 's-phase-fraction'."));

            // Genetics and Patient would be the third call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "Genetics"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "s-phase-fraction")
                )
            )
            .Callback(() => Assert.Equal(3, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Genetics', audience 'Patient', language 'en', pretty URL name 's-phase-fraction'."));

            // Genetics and HealthProfessional would be the fourth call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "Genetics"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "s-phase-fraction")
                )
            )
            .Callback(() => Assert.Equal(4, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Genetics', audience 'HealthProfessional', language 'en', pretty URL name 's-phase-fraction'."));

            // Cancer.gov and Patient would be the last call to the terms query service, returning the term.
            termQueryService.Setup(
                termQSvc => termQSvc.GetByName(
                    It.Is<String>(dictionary => dictionary == "Cancer.gov"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<string>(prettyUrlName => prettyUrlName == "s-phase-fraction")
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetByName("NotSet", AudienceType.Patient, "en", "s-phase-fraction", true);

            // Verify that the expected and actual Term are the same.
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(gsTerm));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetWithFallback_TermsPatient.json")));
            Assert.Equal(expected, actual, new JTokenEqualityComparer());

            // Verify that the service layer is called correctly with the fallback logic:
            // 1) Empty dictionary, Patient
            termQueryService.Verify(
                svc => svc.GetByName("NotSet", AudienceType.Patient, "en", "s-phase-fraction"),
                Times.Once
            );
            // 2) Empty dictionary, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetByName("NotSet", AudienceType.HealthProfessional, "en", "s-phase-fraction"),
                Times.Once
            );
            // 3) Genetics, Patient
            termQueryService.Verify(
                svc => svc.GetByName("Genetics", AudienceType.Patient, "en", "s-phase-fraction"),
                Times.Once
            );
            // 4) Genetics, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetByName("Genetics", AudienceType.HealthProfessional, "en", "s-phase-fraction"),
                Times.Once
            );
            // 5) Cancer.gov, Patient
            termQueryService.Verify(
                svc => svc.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-phase-fraction"),
                Times.Once
            );
        }
    }
}