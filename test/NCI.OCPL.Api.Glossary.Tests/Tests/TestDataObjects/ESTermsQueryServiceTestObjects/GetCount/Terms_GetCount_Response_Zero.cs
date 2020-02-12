
namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class Terms_GetCount_Response_Zero : BaseTermsCountResponseData
    {
        public override string TestFilename => "zero.json";

        public override long ExpectedCount => 0;

    }
}