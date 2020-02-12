
namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    // Just to be clear, ES should *never* return this many. If it ever does, something big is probably wrong. 😉
    public class Terms_GetCount_Response_OMG : BaseTermsCountResponseData
    {
        public override string TestFilename => "omg.json";

        public override long ExpectedCount => 2147483647;

    }
}