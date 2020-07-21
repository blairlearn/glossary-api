using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public abstract class GetAll_Request_Base
    {
        public abstract string Dictionary { get; }

        public abstract AudienceType Audience { get; }

        public abstract string LangCode { get; }

        public abstract int Size { get; }

        public abstract int From { get; }

        public abstract bool IncludeAdditionalInfo { get; }

        public abstract JObject ExpectedRequest { get; }
    }
}