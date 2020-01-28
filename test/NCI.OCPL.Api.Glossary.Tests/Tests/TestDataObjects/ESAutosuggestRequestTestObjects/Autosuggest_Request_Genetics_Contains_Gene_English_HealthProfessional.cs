using NCI.OCPL.Api.Glossary;
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Dictionary - Genetics
    /// Contains - True
    /// SearchText - gene
    /// Language - English
    /// Audience - HealthProfessional
    /// </summary>
    class Autosuggest_Request_Genetics_Contains_Gene_English_HealthProfessional : BaseAutosuggestRequestTestData
    {
        public   override   string SearchText => "gene" ;

        public override  MatchType   Contains => MatchType.Contains;

        public override string DictionaryName => "Genetics";

        public override string Language => "en";

        public override AudienceType Audience => AudienceType.HealthProfessional;

        public override int Size => 10;

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
                                ""match"": {
                                    ""term_name._autocomplete"": {
                                        ""query"": ""gene""
                                    }
                                }
                            }
                        ],
                        ""must_not"": [
                            {
                                ""prefix"": {
                                    ""term_name"": {
                                        ""value"": ""gene""
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
                ""size"": 10
            }
        ");
    }
}