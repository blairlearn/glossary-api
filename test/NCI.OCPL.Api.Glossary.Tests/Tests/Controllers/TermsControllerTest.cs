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
        public async void GetById_ErrorMessage_Dictionary()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("", AudienceType.Patient, "EN", 10L));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Language()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "", 10L));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Id()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "EN", 0L));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_InvalidLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
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

            TermsController controller = new TermsController(termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData.json"));

            // Verify that the service layer is called:
            // a) with the expected values.
            // b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public async void GetById_BlankRequiredFields()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
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

            termsQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.IsAny<String>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<long>()
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(termsQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }
    }
}