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
    public class Autosuggest_Request_Genetics_Begin_Dar_English_HealthProfessional : BaseAutosuggestRequestTestData
    {
        public override string SearchText => "dar";

        public override MatchType Contains => MatchType.Begins;

        public override string DictionaryName => "Genetics";

        public override string Language => "en";

        public override AudienceType Audience => AudienceType.HealthProfessional;

        public override int Size => 35;

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
                                        ""value"": ""Genetics""
                                    }
                                }
                            },
                            {
                                ""prefix"": {
                                    ""term_name"": {
                                        ""value"": ""dar""
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