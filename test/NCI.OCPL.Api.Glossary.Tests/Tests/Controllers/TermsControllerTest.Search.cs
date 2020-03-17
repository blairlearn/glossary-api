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
        public async void Search_ErrorMessage_Dictionary()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" })
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }
        [Fact]
        public async void Search_ErrorMessage_EmptyLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("Cancer.gov", AudienceType.Patient, "", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" })
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }

        [Fact]
        public async void Search_ErrorMessage_InvalidLanguage()
        {
            Mock<ITermsQueryService> termQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("Cancer.gov", AudienceType.Patient, "chicken", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" })
            );
            Assert.Equal("Unsupported Language. Valid values are 'en' and 'es'.", exception.Message);
        }

        [Fact]
        public async void Search_ErrorMessage_AudienceType(){
            Mock<ITermsQueryService> termsQueryService = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termsQueryService.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("Cancer.gov", (AudienceType)(-18), "en", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" })
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }

        /// <summary>
        /// Verify that Search behaves in the expected manner when only required parameters are passed in.
        /// </summary>
        [Fact]
        public async void Search_RequiredParametersOnly()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken");

            // Verify that the query layer is called:
            //  a) with the expected updated values for size, from, and requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 100, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once,
                "ITermsQueryService::Search() should be called once, with the updated value for size"
            );
        }

        /// <Summary>
        /// Verify that Search behaves in the expected manner when size is an invalid value.
        /// </Summary>
        [Fact]
        public async void Search_InvalidMatchType()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", (MatchType)5, -1, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" })
            );
            Assert.Equal("The `matchType` parameter must be either 'Begins' or 'Contains'.", exception.Message);
        }

        /// <Summary>
        /// Verify that Search behaves in the expected manner when size is an invalid value.
        /// </Summary>
        [Fact]
        public async void Search_InvalidSize()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, -1, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" });

            // Verify that the query layer is called:
            //  a) with the expected updated values for size.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 100, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once,
                "ITermsQueryService::Search() should be called once, with the updated value for size"
            );
        }

        /// <summary>
        /// Verify that Search behaves in the expected manner when from is an invalid value.
        /// </summary>
        [Fact]
        public async void Search_InvalidFrom()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, -1, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" });

            // Verify that the query layer is called:
            //  a) with the expected updated values for from and size.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once,
                "ITermsQueryService::Search() should be called once, with the updated value for from"
            );
        }

        /// <summary>
        /// Verify that Search behaves in the expected manner when requestedFields is null.
        /// </summary>
        [Fact]
        public async void Search_NullRequestedFields()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, -1, null);

            // Verify that the query layer is called:
            //  a) with the expected updated values for requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once,
                "ITermsQueryService::Search() should be called once, with the updated value for requestedFields"
            );
        }

        /// <summary>
        /// Verify that Search behaves in the expected manner when requestedFields is invalid
        /// by having any array items that are null.
        /// </summary>
        [Fact]
        public async void Search_InvalidRequestedFields()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{null, null, null});

            // Verify that the query layer is called:
            //  a) with the expected updated values for requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once,
                "ITermsQueryService::Search() should be called once, with the updated value for requestedFields"
            );
        }

        /// <summary>
        /// Verify that Search behaves in the expected manner when requestedFields is an empty array.
        /// </summary>
        [Fact]
        public async void Search_EmptyRequestedFields()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = getDumbSearchSvcMock();

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, querySvc.Object);
            await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{});

            // Verify that the query layer is called:
            //  a) with the expected updated values for requestedFields.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once,
                "ITermsQueryService::Search() should be called once, with the updated value for requestedFields"
            );
        }

        /// <summary>
        /// Verify that Search endpoint makes the correct call to the query service,
        /// and returns the correct response for the specified parameters.
        /// </summary>
        [Fact]
        public async void Search_HandlesResults()
        {
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

            Mock<ITermsQueryService> termsQueryService = getDumbSearchSvcMock(glossaryTermResults);
            TermsController controller = new TermsController(NullLogger<TermsController>.Instance, termsQueryService.Object);
            string[] requestedFields = new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" };


            GlossaryTermResults termResults = await controller.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 5, 0, requestedFields );
            JObject actual = JObject.Parse(JsonConvert.SerializeObject(termResults));
            JObject expected = JObject.Parse(File.ReadAllText(TestingTools.GetPathToTestFile("TermsControllerData/TestData_Expand.json")));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termsQueryService.Verify(
                svc => svc.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 5, 0, new string[]{ "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" }),
                Times.Once
            );

            Assert.Equal(glossaryTermResults.Results, termResults.Results, new GlossaryTermComparer());
            Assert.Equal(glossaryTermResults.Meta.TotalResults, termResults.Meta.TotalResults);
            Assert.Equal(glossaryTermResults.Meta.From, termResults.Meta.From);
            Assert.Equal(expected, actual, new JTokenEqualityComparer());
        }

        /// <summary>
        /// Gets a TermsQueryService mock for the search endpoint, returning an empty terms
        /// results.
        /// </summary>
        /// <returns>The Mock</returns>
        private Mock<ITermsQueryService> getDumbSearchSvcMock(GlossaryTermResults results = null) {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.Search(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MatchType>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(results == null ? new GlossaryTermResults() : results));
            return querySvc;
        }
    }
}