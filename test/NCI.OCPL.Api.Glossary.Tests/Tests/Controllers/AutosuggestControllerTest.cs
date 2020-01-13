using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Controllers;
using Newtonsoft.Json;
using Xunit;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public class AutosuggestControllerTest
    {
        [Fact]
        public async void GetSuggestions_ErrorMessage_DictionaryMissing()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.getSuggestions("", "HealthProfessional", "en", "suggest"));
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_LanguageMissing()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.getSuggestions("glossary", "HealthProfessional", "", "suggest"));
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_LanguageBad()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.getSuggestions("glossary", "Patient", "invalid", "suggest"));
            Assert.Equal("Unsupported Language. Please try either 'en' or 'es'", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_AudienceType(){
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.getSuggestions("Dictionary", "InvalidValue", "EN", "Query"));
            Assert.Equal("'AudienceType' can  be 'Patient' or 'HealthProfessional' only", exception.Message);
        }

        [Fact]
        public async void SearchForTerms()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            string[] requestedFields = new string[]{"TermName","Pronunciation","Definition"};
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
            List<GlossaryTerm> glossaryTermList = new List<GlossaryTerm>();
            glossaryTermList.Add(glossaryTerm);
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.getSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .Returns(Task.FromResult(glossaryTermList));

            GlossaryTerm[] gsTerm = await controller.getSuggestions("Dictionary", "Patient", "EN", "Query");
            string actualJsonValue = JsonConvert.SerializeObject(
                gsTerm
            );
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData_SearchForTerms.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.getSuggestions("Dictionary", AudienceType.Patient, "EN", "Query"),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }
    }
}