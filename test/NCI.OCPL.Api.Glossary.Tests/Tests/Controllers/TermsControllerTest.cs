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
        public async void GetById_ErrorMessage_Language()
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
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData.json"));

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
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData.json"));

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
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_SearchForTerms.json"));

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
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_SearchForTerms.json"));

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
        public async void Expand_ErrorMessage_Dictionary()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Expand("", AudienceType.Patient, "en", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"})
            );
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }
        [Fact]
        public async void Expand_ErrorMessage_EmptyLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Expand("Cancer.gov", AudienceType.Patient, "", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"})
            );
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }

        [Fact]
        public async void Expand_ErrorMessage_InvalidLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Expand("Cancer.gov", AudienceType.Patient, "chicken", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"})
            );
            Assert.Equal("Unsupported Language. Please try either 'en' or 'es'", exception.Message);
        }

        [Fact]
        public async void Expand_ErrorMessage_AudienceType(){
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Expand("Cancer.gov", (AudienceType)(-18), "en", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"})
            );
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }

        /// <summary>
        /// Verify that Expand behaves in the expected manner when only required parameters are passed in.
        /// </summary>
        [Fact]
        public async void Expand_RequiredParametersOnly()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s");

            // Verify that the query layer is called:
            //  a) with the expected updated values for size, from, and requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 100, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once,
                "ITermsQueryService::Expand() should be called once, with the updated value for size"
            );
        }

        /// <Summary>
        /// Verify that Expand behaves in the expected manner when size is an invalid value.
        /// </Summary>
        [Fact]
        public async void Expand_InvalidSize()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s", -1, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"});

            // Verify that the query layer is called:
            //  a) with the expected updated values for size.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 100, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once,
                "ITermsQueryService::Expand() should be called once, with the updated value for size"
            );
        }

        /// <summary>
        /// Verify that Expand behaves in the expected manner when from is an invalid value.
        /// </summary>
        [Fact]
        public async void Expand_InvalidFrom()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, -1, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"});

            // Verify that the query layer is called:
            //  a) with the expected updated values for from and size.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once,
                "ITermsQueryService::Expandl() should be called once, with the updated value for from"
            );
        }

        /// <summary>
        /// Verify that Expand behaves in the expected manner when requestedFields is null.
        /// </summary>
        [Fact]
        public async void Expand_NullRequestedFields()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, -1, null);

            // Verify that the query layer is called:
            //  a) with the expected updated values for requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once,
                "ITermsQueryService::Expandl() should be called once, with the updated value for requestedFields"
            );
        }

        /// <summary>
        /// Verify that Expand behaves in the expected manner when requestedFields is invalid
        /// by having any array items that are null.
        /// </summary>
        [Fact]
        public async void Expand_InvalidRequestedFields()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, 0, new string[]{null, null, null});

            // Verify that the query layer is called:
            //  a) with the expected updated values for requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once,
                "ITermsQueryService::Expandl() should be called once, with the updated value for requestedFields"
            );
        }

        /// <summary>
        /// Verify that Expand behaves in the expected manner when requestedFields is an empty array.
        /// </summary>
        [Fact]
        public async void Expand_EmptyRequestedFields()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, 0, new string[]{});

            // Verify that the query layer is called:
            //  a) with the expected updated values for requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 10, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once,
                "ITermsQueryService::Expandl() should be called once, with the updated value for requestedFields"
            );
        }

        /// <summary>
        /// Verify that Expand endpoint makes the correct call to the query service,
        /// and returns the correct response for the specified parameters.
        /// </summary>
        [Fact]
        public async void ExpandTerms()
        {
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(termsQueryService.Object);
            string[] requestedFields = new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"};

            GlossaryTermResults glossaryTermResults = new GlossaryTermResults() {
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

            termsQueryService.Setup(
                termQSvc => termQSvc.Expand(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(glossaryTermResults));

            GlossaryTermResults termResults = await controller.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 5, 0, requestedFields );
            string actualJsonValue = JsonConvert.SerializeObject(termResults);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_Expand.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 5, 0, new string[]{"term_id", "language", "dictionary", "audience", "term_name", "first_letter", "pretty_url_name", "definition", "pronunciation"}),
                Times.Once
            );

            Assert.Equal(glossaryTermResults.Results, termResults.Results, new GlossaryTermComparer());
            Assert.Equal(glossaryTermResults.Meta.TotalResults, termResults.Meta.TotalResults);
            Assert.Equal(glossaryTermResults.Meta.From, termResults.Meta.From);
            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

    }
}