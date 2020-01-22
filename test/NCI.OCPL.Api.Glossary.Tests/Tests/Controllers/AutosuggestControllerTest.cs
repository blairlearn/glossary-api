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
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetSuggestions("", "HealthProfessional", "en", "suggest", true, 20, 0));
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_LanguageMissing()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetSuggestions("glossary", "HealthProfessional", "", "suggest", false, 20, 0));
            Assert.Equal("You must supply a valid dictionary, audience and language", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_LanguageBad()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetSuggestions("glossary", "Patient", "invalid", "suggest", true, 20, 0));
            Assert.Equal("Unsupported Language. Please try either 'en' or 'es'", exception.Message);
        }

        [Fact]
        public async void GetSuggestions_ErrorMessage_AudienceType()
        {
            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);
            APIErrorException exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetSuggestions("Dictionary", "InvalidValue", "EN", "Query", true, 20, 0));
            Assert.Equal("'AudienceType' can  be 'Patient' or 'HealthProfessional' only", exception.Message);
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
            const bool contains = true;
            const int resultSize = 100;
            const int resultsFrom = 200;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience.ToString(), language, queryText, contains, resultSize, resultsFrom);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, contains, resultSize, resultsFrom),
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

            const bool expectedContainsRequest = false;
            const int expecedSizeRequest = 20;
            const int expectedFromRequest = 0;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience.ToString(), language, queryText);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, expectedContainsRequest, expecedSizeRequest, expectedFromRequest),
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
            const bool contains = true;
            const int invalidSize = -200;
            const int invalidFrom = -100;

            const int expecedSizeRequest = 20;
            const int expectedFromRequest = 0;

            Mock<IAutosuggestQueryService> querySvc = new Mock<IAutosuggestQueryService>();
            AutosuggestController controller = new AutosuggestController(querySvc.Object);

            // Set up the mock query service to return an empty array.
            querySvc.Setup(
                autoSuggestQSvc => autoSuggestQSvc.GetSuggestions(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience.ToString(), language, queryText, contains, invalidSize, invalidFrom);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, contains, expecedSizeRequest, expectedFromRequest),
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
            const bool contains = false;
            const int invalidSize = 0;
            const int from = 10;

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
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .Returns(Task.FromResult(new Suggestion[] { }));

            Suggestion[] result = await controller.GetSuggestions(dictionary, audience.ToString(), language, queryText, contains, invalidSize, from);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetSuggestions(dictionary, audience, language, queryText, contains, expecedSizeRequest, from),
                Times.Once
            );

            Assert.Empty(result);
        }

    }
}