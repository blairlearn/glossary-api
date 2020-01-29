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
    public class TermsControllerTests
    {
[JsonProperty("GlossaryTerm")]
        private GlossaryTerm glossaryTerm;

        private GlossaryTerm GetGlossaryTerm()
        {
            return glossaryTerm;
        }

        private void SetGlossaryTerm(GlossaryTerm value)
        {
            glossaryTerm = value;
        }

        [Fact]
        public async void GetById_ErrorMessage_Dictionary()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("", AudienceType.Patient, "EN", 10L, new string[] { }));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Languate()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "", 10L, new string[] { }));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Id()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "EN", 0L, new string[] { }));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            string[] requestedFields = { "TermName", "Pronunciation", "Definition" };
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
                    It.IsAny<long>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));

            // Verify that the service layer is called:
            // a) with the expected values.
            // b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, new string[] { "TermName", "Pronunciation", "Definition" }),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public async void GetById_BlankRequiredFields()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            string[] requestedFields = new string[] { };
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
                    It.IsAny<long>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(glossaryTerm));

            TermsController controller = new TermsController(termsQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, new string[] { "TermName", "Pronunciation", "Definition" }),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public async void GetAll_Error_DictionaryMissing()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();

            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetAll("", AudienceType.HealthProfessional, "en"));
        }

        [Fact]
        public async void GetAll_Error_LanguageMissing()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();

            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetAll("glossary", AudienceType.HealthProfessional, ""));
        }

        [Fact]
        public async void GetAll_Error_LanguageBad()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();

            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetAll("glossary", AudienceType.HealthProfessional, "turducken"));
        }

        /// <Summary>
        /// Verify that getAll behaves in the expected manner when size, from, and requestedFields
        /// aren't set in the call.
        /// </Summary>
        [Fact]
        public async void GetAll_Default_Parameters()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.GetAll(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.GetAll("glossary", AudienceType.HealthProfessional, "en");

            // Verify that the query layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetAll("glossary", AudienceType.HealthProfessional, "en", 10, 0, new string[] { "TermName", "Pronunciation", "Definition" }),
                Times.Once,
                "ITermsQueryService::getAll() should be called once, with default values for size, from, and requestedFields"
            );
        }

        /// <Summary>
        /// Verify that getAll behaves in the expected manner when size, from, and requestedFields
        /// are set to explicit values
        /// </Summary>
        [Fact]
        public async void GetAll_Specified_Parameters()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.GetAll(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.GetAll("glossary", AudienceType.HealthProfessional, "es", 200, 2, new string[] {"Field1", "Field2", "Field3"});

            // Verify that the query layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetAll("glossary", AudienceType.HealthProfessional, "es", 200, 2, new string[] { "Field1", "Field2", "Field3" }),
                Times.Once,
                "ITermsQueryService::getAll() should be called once, with the specified values for size, from, and requestedFields"
            );
        }

        [Fact]
        public async void SearchForTerms_ErrorMessage_MatchType(){
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("Dictionary", AudienceType.Patient, "EN", "Query", "doesnotcontain",1,1,new string[]{})
            );
            Assert.Equal("'matchType' can only be 'begins' or 'contains'", exception.Message);
        }

        [Fact]
        public async void SearchForTerms_ErrorMessage_AudienceType(){
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("Dictionary", (AudienceType)(-18), "EN", "Query", "contains",0,1,new string[]{})
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }


        [Fact]
        public async void SearchForTerms()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            string[] requestedFields = new string[]{};
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
            termsQueryService.Setup(
                termQSvc => termQSvc.Search(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(glossaryTermList));

            GlossaryTerm[] gsTerm = await controller.Search("Dictionary", AudienceType.Patient, "EN", "Query", "contains",1,0,new string[] {"TermName","Pronunciation","Definition"});
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData_SearchForTerms.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.Search("Dictionary", AudienceType.Patient, "EN", "Query", "contains",1,0, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public async void SearchForTermsWithsize()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            string[] requestedFields = new string[] {"TermName","Pronunciation","Definition"};
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
            int sizeValue = 100;
            int fromValue = 0;
            termsQueryService.Setup(
                termQSvc => termQSvc.Search(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    sizeValue,
                    fromValue,
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(glossaryTermList));

            GlossaryTerm[] gsTerm = await controller.Search("Dictionary", AudienceType.Patient, "EN", "Query", "contains",0,0,requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData_SearchForTerms.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.Search("Dictionary", AudienceType.Patient, "EN", "Query", "contains",100,0, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public async void ExpandTerms()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
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
            termsQueryService.Setup(
                termQSvc => termQSvc.Search(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(glossaryTermList));

            GlossaryTerm[] gsTerm = await controller.Search("Dictionary", AudienceType.Patient, "EN", "Query", "contains",1,0,requestedFields );
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData_SearchForTerms.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.Search("Dictionary", AudienceType.Patient, "EN", "Query", "contains",1,0, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

    }
}