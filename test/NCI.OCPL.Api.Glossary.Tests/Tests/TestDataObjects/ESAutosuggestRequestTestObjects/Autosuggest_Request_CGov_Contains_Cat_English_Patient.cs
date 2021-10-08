using NCI.OCPL.Api.Glossary;
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Dictionary - Cancer.gov
    /// Contains - True
    /// SearchText - cat
    /// Language - Spanish
    /// Audience - Patient
    /// </summary>
    class Autosuggest_Request_CGov_Contains_Cat_English_Patient : BaseAutosuggestRequestTestData
    {
        public   override   string SearchText => "cat" ;

        public override  MatchType   MatchType => MatchType.Contains;

        public override string DictionaryName => "Cancer.gov";

        public override string Language => "en";

        public override AudienceType Audience => AudienceType.Patient;

        public override int Size => 14;

        public override JObject ExpectedData => JObject.Parse(@"
            {
                ""query"": {
                    ""bool"": {
                        ""must"": [
                            {
                                ""term"": {
                                    ""language"": {
                                        ""value"": ""en""
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
                                ""match_phrase"": {
                                    ""term_name._autocomplete"": {
                                        ""query"": ""cat""
                                    }
                                }
                            }
                        ],
                        ""must_not"": [
                            {
                                ""prefix"": {
                                    ""term_name"": {
                                        ""value"": ""cat""
                                    }
                                }
                            }
                        ]
                    }
                },
                ""sort"": [
                    { ""term_name"": {} }
                ],
                ""_source"": {
                    ""includes"": [
                        ""term_id"",
                        ""term_name""
                    ]
                },
                ""size"": 14
            }
        ");
    }
}