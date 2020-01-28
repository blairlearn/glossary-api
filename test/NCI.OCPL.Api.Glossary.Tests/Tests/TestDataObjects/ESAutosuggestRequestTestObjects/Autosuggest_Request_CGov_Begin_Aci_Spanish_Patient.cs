using NCI.OCPL.Api.Glossary;
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Dictionary - Cancer.gov
    /// Contains - False
    /// SearchText - aci
    /// Language - Spanish
    /// Audience - Patient
    /// </summary>
    public class Autosuggest_Request_CGov_Begin_Aci_Spanish_Patient : BaseAutosuggestRequestTestData
    {
        public override string SearchText => "aci";

        public override MatchType Contains => MatchType.Begins;

        public override string DictionaryName => "Cancer.gov";

        public override string Language => "es";

        public override AudienceType Audience => AudienceType.Patient;

        public override int Size => 35;

        public override JObject ExpectedData => JObject.Parse(@"
            {
                ""query"": {
                    ""bool"": {
                        ""must"": [{
                                ""term"": {
                                    ""language"": {
                                        ""value"": ""es""
                                    }
                                }
                            },
                            {
                                ""term"": {
                                    ""audience"": {
                                        ""value"": ""Patient""
                                    }
                                }
                            },
                            {
                                ""term"": {
                                    ""dictionary"": {
                                        ""value"": ""Cancer.gov""
                                    }
                                }
                            },
                            {
                                ""prefix"": {
                                    ""term_name"": {
                                        ""value"": ""aci""
                                    }
                                }
                            }
                        ]
                    }
                },
                ""size"": 35,
                ""_source"": {
                    ""includes"": [
                        ""term_id"",
                        ""term_name""
                    ]
                },
                ""sort"": [
                    { ""term_name"": {} }
                ]
            }
        ");
    }
}