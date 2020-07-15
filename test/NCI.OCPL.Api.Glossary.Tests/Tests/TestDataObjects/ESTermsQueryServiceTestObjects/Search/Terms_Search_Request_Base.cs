using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public abstract class Terms_Search_Request_Base
    {
        public abstract string Dictionary { get; }

        public abstract AudienceType Audience { get; }

        public abstract string LangCode { get; }

        public abstract string SearchTerm { get; }

        public abstract MatchType MatchType { get; }

        public abstract int Size { get; }

        public abstract int From { get; }

        public abstract string[] FieldList { get; }

        public abstract JObject ExpectedRequest { get; }
    }
}