using System;
using System.Threading.Tasks;

using Moq;
using Xunit;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary.Controllers;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public class TermsController_Count_Test
    {
        /// <summary>
        /// Verify an error is thrown when no dictionary is specified.
        /// </summary>
        [Theory]
        [InlineData(new object[] { null })]
        [InlineData(new object[] { "" })]  // Empty string.
        public async void Count_ErrorMessage_Dictionary(string dictionary)
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetCount(dictionary, AudienceType.HealthProfessional, "en")
            );
            Assert.Equal(400, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify an error is thrown when an invalid audience is specified.
        /// </summary>
        [Fact]
        public async void Count_ErrorMessage_Audience()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetCount("Cancer.gov", (AudienceType)6, "en")
            );
            Assert.Equal(400, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify an error is thrown when an no language is specified.
        /// </summary>
        [Theory]
        [InlineData(new object[] { null })]
        [InlineData(new object[] { "" })]  // Empty string.
        public async void Count_ErrorMessage_Language(string badLanguage)
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => controller.GetCount("Cancer.gov", AudienceType.Patient, badLanguage)
            );
            Assert.Equal(400, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify that controller arguments are passed to the query service as expected.
        /// </summary>
        [Theory]
        [InlineData("Cancer.gov", AudienceType.HealthProfessional, "en")]
        [InlineData("Cancer.gov", AudienceType.HealthProfessional, "es")]
        [InlineData("Cancer.gov", AudienceType.Patient, "en")]
        [InlineData("Cancer.gov", AudienceType.Patient, "es")]
        [InlineData("Genetics", AudienceType.HealthProfessional, "en")]
        [InlineData("Genetics", AudienceType.HealthProfessional, "es")]
        [InlineData("Genetics", AudienceType.Patient, "en")]
        [InlineData("Genetics", AudienceType.Patient, "es")]
        public async void Count_QueryValues(string dictionary, AudienceType audience, string language)
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(querySvc.Object);

            // Set up the mock query service to handle the GetCount() call.
            // The return value is unimportant for this test.
            querySvc.Setup(
                mock => mock.GetCount(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>()
                )
            )
            .Returns(Task.FromResult(default(long)));

            long result = await controller.GetCount(dictionary, audience, language);

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.GetCount(dictionary, audience, language),
                Times.Once
            );
        }

        /// <summary>
        /// Verify that the controller correctly returns the value from the query service.
        /// </summary>
        [Theory]
        [InlineData(new object[] { 0 })]
        [InlineData(new object[] { 3 })]
        [InlineData(new object[] { 20 })]
        [InlineData(new object[] { 100 })]
        [InlineData(new object[] { 500 })]
        [InlineData(new object[] { int.MaxValue })] // Utterly ridiculous.
        public async void Count_QueryReturn(long returnCount)
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            TermsController controller = new TermsController(querySvc.Object);

            // Set up the mock query service to handle the GetCount() call.
            // The return value is unimportant for this test.
            querySvc.Setup(
                mock => mock.GetCount(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>()
                )
            )
            .Returns(Task.FromResult(returnCount));

            // The arguments don't matter for this test.
            long actual = await controller.GetCount("Cancer.gov", AudienceType.HealthProfessional, "es");
            Assert.Equal(returnCount, actual);
        }
    }
}