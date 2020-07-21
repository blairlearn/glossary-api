using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public abstract class ExpandRequestBase
    {
        public abstract string Dictionary { get; }

        public abstract AudienceType Audience { get; }

        public abstract string LanguageCode { get; }

        public abstract string ExpandCharacter { get; }

        public abstract int Size { get; }

        public abstract int From { get; }

        public abstract bool IncludeAdditionalInfo { get; }

        public abstract JObject ExpectedRequest { get; }
    }
}