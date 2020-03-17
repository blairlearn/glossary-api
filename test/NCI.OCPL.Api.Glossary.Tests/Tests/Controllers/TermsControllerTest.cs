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
        public async void GetById_ErrorMessage_Dictionary()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("", AudienceType.Patient, "EN", 10L));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Language()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "", 10L));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Id()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "EN", 0L));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_InvalidLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Cancer.gov", AudienceType.Patient, "chicken", 10L)
            );
            Assert.Equal("Unsupported Language. Please try either 'en' or 'es'", exception.Message);
        }

        [Fact]
        public async void GetById()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            Pronunciation pronunciation = new Pronunciation("Pronunciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            GlossaryTerm glossaryTerm = new GlossaryTerm
            {
                TermId = 1234L,
                Language = "EN",
                Dictionary = "Dictionary",
                Audience = AudienceType.Patient,
                TermName = "TermName",
                FirstLetter = "t",
                PrettyUrlName = "www.glossary-api.com",
                Pronunciation = pronunciation,
                Definition = definition,
                RelatedResources = new IRelatedResource[] {
                    new LinkResource()
                    {
                        Type = RelatedResourceType.External,
                        Text = "Link to Google",
                        Url = new System.Uri("https://www.google.com")
                    },
                    new LinkResource()
                    {
                        Type = RelatedResourceType.DrugSummary,
                        Text = "Bevacizumab",
                        Url = new System.Uri("https://www.cancer.gov/about-cancer/treatment/drugs/bevacizumab")
                    },
                    new LinkResource()
                    {
                        Type = RelatedResourceType.Summary,
                        Text = "Lung cancer treatment",
                        Url = new System.Uri("https://www.cancer.gov/types/lung/patient/small-cell-lung-treatment-pdq")
                    },
                    new GlossaryResource()
                    {
                        Type = RelatedResourceType.GlossaryTerm,
                        Text = "stage II cutaneous T-cell lymphoma",
                        TermId = 43966,
                        Audience = AudienceType.Patient,
                        PrettyUrlName = "stage-ii-cutaneous-t-cell-lymphoma"
                    }
                }
            };
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.IsAny<String>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<long>()
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L);
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(gsTerm));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetById.json")));

            // Verify that the service layer is called:
            // a) with the expected values.
            // b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L),
                Times.Once
            );

            Assert.Equal(expected, actual, new JTokenEqualityComparer());
        }

        // This test is for the HealthProfessional fallback logic for GetById.
        // The expected returns from the terms query service for the calls expected to be made are set up.
        // The call order of the terms query service calls are set up.
        // It performs the call to the controller with the correct params.
        // It verifies the call order of the terms query service calls.
        // It verifies that the expected and actual glossaryTerm objects are the same.
        // It verifies that the query service was called the expected number of times with the expected params.
        [Fact]
        public async void GetById_WithFallback_GeneticsHP()
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
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "Cancer.gov"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 556486)
                )
            )
            .Callback(() => Assert.Equal(1, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Cancer.gov', audience 'HealthProfessional', language 'en' and id '556486'."));

            // Cancer.gov and Patient would be the second call to the terms query service, returning nothing.
            termQueryService.Setup(termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "Cancer.gov"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 556486)
                )
            )
            .Callback(() => Assert.Equal(2, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Cancer.gov', audience 'Patient', language 'en' and id '556486'."));

            // NotSet and HealthProfessional would be the third call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 556486)
                )
            )
            .Callback(() => Assert.Equal(3, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'HealthProfessional', language 'en' and id '556486'."));

            // NotSet and Patient would be the fourth call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 556486)
                )
            )
            .Callback(() => Assert.Equal(4, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'Patient', language 'en' and id '556486'."));

            // Genetics and HealthProfessional would be the last call to the terms query service, returning the term.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "Genetics"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 556486)
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Cancer.gov", AudienceType.HealthProfessional, "en", 556486, true);

            // Verify that the expected and actual Term are the same.
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(gsTerm));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetWithFallback_GeneticsHP.json")));
            Assert.Equal(expected, actual, new JTokenEqualityComparer());

            // Verify that the service layer is called correctly with the fallback logic:
            // 1) Cancer.gov, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetById("Cancer.gov", AudienceType.HealthProfessional, "en", 556486),
                Times.Once
            );
            // 2) Cancer.gov, Patient
            termQueryService.Verify(
                svc => svc.GetById("Cancer.gov", AudienceType.Patient, "en", 556486),
                Times.Once
            );
            // 3) Empty dictionary, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetById("NotSet", AudienceType.HealthProfessional, "en", 556486),
                Times.Once
            );
            // 4) Empty dictionary, Patient
            termQueryService.Verify(
                svc => svc.GetById("NotSet", AudienceType.Patient, "en", 556486),
                Times.Once
            );
            // 5) Genetics, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetById("Genetics", AudienceType.HealthProfessional, "en", 556486),
                Times.Once
            );
        }

        // This test is for the Patient fallback logic for GetById.
        // The expected returns from the terms query service for the calls expected to be made are set up.
        // The call order of the terms query service calls are set up.
        // It performs the call to the controller with the correct params.
        // It verifies the call order of the terms query service calls.
        // It verifies that the expected and actual glossaryTerm objects are the same.
        // It verifies that the query service was called the expected number of times with the expected params.
        [Fact]
        public async void GetById_WithFallback_TermsPatient()
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
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 44771)
                )
            )
            .Callback(() => Assert.Equal(1, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'Patient', language 'en' and id '44771'."));

            // NotSet and HealthProfessional would be the second call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "NotSet"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 44771)
                )
            )
            .Callback(() => Assert.Equal(2, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'NotSet', audience 'HealthProfessional', language 'en' and id '44771'."));

            // Genetics and Patient would be the third call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "Genetics"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 44771)
                )
            )
            .Callback(() => Assert.Equal(3, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Genetics', audience 'Patient', language 'en' and id '44771'."));

            // Genetics and HealthProfessional would be the fourth call to the terms query service, returning nothing.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "Genetics"),
                    It.Is<AudienceType>(audience => audience == AudienceType.HealthProfessional),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 44771)
                )
            )
            .Callback(() => Assert.Equal(4, callOrder++))
            .Throws(new APIErrorException(200, "Empty response when searching for dictionary 'Genetics', audience 'HealthProfessional', language 'en' and id '44771'."));

            // Cancer.gov and Patient would be the last call to the terms query service, returning the term.
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.Is<String>(dictionary => dictionary == "Cancer.gov"),
                    It.Is<AudienceType>(audience => audience == AudienceType.Patient),
                    It.Is<string>(language => language == "en"),
                    It.Is<long>(id => id == 44771)
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("NotSet", AudienceType.Patient, "en", 44771, true);

            // Verify that the expected and actual Term are the same.
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(gsTerm));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_GetWithFallback_TermsPatient.json")));
            Assert.Equal(expected, actual, new JTokenEqualityComparer());

            // Verify that the service layer is called correctly with the fallback logic:
            // 1) Empty dictionary, Patient
            termQueryService.Verify(
                svc => svc.GetById("NotSet", AudienceType.Patient, "en", 44771),
                Times.Once
            );
            // 2) Empty dictionary, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetById("NotSet", AudienceType.HealthProfessional, "en", 44771),
                Times.Once
            );
            // 3) Genetics, Patient
            termQueryService.Verify(
                svc => svc.GetById("Genetics", AudienceType.Patient, "en", 44771),
                Times.Once
            );
            // 4) Genetics, HealthProfessional
            termQueryService.Verify(
                svc => svc.GetById("Genetics", AudienceType.HealthProfessional, "en", 44771),
                Times.Once
            );
            // 5) Cancer.gov, Patient
            termQueryService.Verify(
                svc => svc.GetById("Cancer.gov", AudienceType.Patient, "en", 44771),
                Times.Once
            );
        }
    }
}