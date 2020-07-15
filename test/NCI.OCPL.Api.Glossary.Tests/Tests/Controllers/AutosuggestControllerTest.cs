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
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetSuggestions("", AudienceType.HealthProfessional, "en", "suggest", MatchType.Contains, 20)
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_LanguageMissing()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetSuggestions("glossary", AudienceType.HealthProfessional, "", "suggest", MatchType.Begins, 20)
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_LanguageBad()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetSuggestions("glossary", AudienceType.Patient, "invalid", "suggest", MatchType.Contains, 20)
            );
            Assert.Equal("Unsupported Language. Valid values are 'en' and 'es'.", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_AudienceType()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetSuggestions("Dictionary", (AudienceType)(-20937), "EN", "Query", MatchType.Contains, 20)
            );
            Assert.Equal("You must supply a valid dictionary, audience and language.", exception.Message);
        }

        /// <summary>
        /// Verify that explicit values passed to the controller are passed in turn to the query service.
        /// </summary>
        [Fact]
        public async void Verify_Explicit_Values_Passed_to_Service()
        {
            const string dictionary = "Cancer.gov";
            const AudienceType audience = AudienceType.Patient;
            const string language = "en";
            const string queryText = "chicken";
            const MatchType matchType = MatchType.Contains;
            const int resultSize = 100;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MatchType>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience, language, queryText, matchType, resultSize);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, matchType, resultSize),
                Times.Once
            );

            Assert.Empty(result);
        }

        /// <summary>
        /// Verify the correct defaults are passed to the query service when no valies are specified for beginsWith, size, and from.
        /// </summary>
        [Fact]
        public async void Verify_Default_Values_Passed_to_Service()
        {
            const string dictionary = "Cancer.gov";
            const AudienceType audience = AudienceType.Patient;
            const string language = "en";
            const string queryText = "chicken";

            const MatchType expectedMatchType = MatchType.Begins;
            const int expecedSizeRequest = 20;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MatchType>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience, language, queryText);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, expectedMatchType, expecedSizeRequest),
                Times.Once
            );

            Assert.Empty(result);
        }

        /// <summary>
        /// Verify that negative values for size and from are properly handled before the service is invoked.
        /// </summary>
        [Fact]
        public async void Verify_Negative_Value_Handling()
        {
            const string dictionary = "Cancer.gov";
            const AudienceType audience = AudienceType.Patient;
            const string language = "en";
            const string queryText = "chicken";
            const MatchType matchType = MatchType.Contains;
            const int invalidSize = -200;

            const int expecedSizeRequest = 20;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MatchType>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience, language, queryText, matchType, invalidSize);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, matchType, expecedSizeRequest),
                Times.Once
            );

            Assert.Empty(result);
        }

        /// <summary>
        /// Verify that passing zero for the  size argument is properly handled before the service is invoked.
        /// </summary>
        [Fact]
        public async void Verify_Zero_Size_Handling()
        {
            const string dictionary = "Cancer.gov";
            const AudienceType audience = AudienceType.Patient;
            const string language = "en";
            const string queryText = "chicken";
            const MatchType matchType = MatchType.Begins;
            const int invalidSize = 0;

            const int expecedSizeRequest = 20;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MatchType>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience, language, queryText, matchType, invalidSize);

            // Verify that the service layer is called:
            //  a) with the exact values passed to the controller.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, matchType, expecedSizeRequest),
                Times.Once
            );

            Assert.Empty(result);
        }

        /// <summary>
        /// Verify search parameters passed to the controller are passed to the query wihtout modificiation.
        /// </summary>
        [Theory]
        [InlineData("Cancer.gov", AudienceType.HealthProfessional, "en", "chicken",          MatchType.Begins)]
        [InlineData("Cancer.gov", AudienceType.Patient,            "es", "pollo",            MatchType.Contains)]
        [InlineData("Genetics",  AudienceType.HealthProfessional,  "en", "Are you kidding?", MatchType.Exact)]
        public async void Verify_Argument_passing(string dictionary, AudienceType audience, string language, string queryText, MatchType matchType)
        {
            const int expecedSizeRequest = 20;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MatchType>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience, language, queryText, matchType);

            // Verify that the service layer is called:
            //  a) with the exact values passed to the controller.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, matchType, expecedSizeRequest),
                Times.Once
            );

            Assert.Empty(result);
        }
    }
}