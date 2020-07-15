using NCI.OCPL.Api.Glossary;
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Dictionary - Genetics
    /// Contains - False
    /// SearchText - dar
    /// Language - English
    /// Audience - HealthProfessional
    /// </summary>
    public class Autosuggest_Request_Exact : BaseAutosuggestRequestTestData
    {
        public override string SearchText => "Are you kidding?";

        public override MatchType MatchType => MatchType.Exact;

        public override string DictionaryName => "Cancer.gov";

        public override string Language => "en";

        public override AudienceType Audience => AudienceType.HealthProfessional;

        public override int Size => 1;

        public override JObject ExpectedData => JObject.Parse(@"
            {
                ""query"": {
                    ""bool"": {
                        ""must"": [{
                                ""term"": {
                                    ""language"": {
                                        ""value"": ""en""
                                    }
                                }
                            },
                            {
                                ""term"": {
                                    ""audience"": {
                                        ""value"": ""HealthProfessional""
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
                                ""term"": {
                                    ""term_name"": {
                                        ""value"": ""Are you kidding?""
                                    }
                                }
                            }
                        ]
                    }
                },
                ""size"": 1,
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