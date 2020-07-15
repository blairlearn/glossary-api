
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    class Terms_Search_Request_Exact : Terms_Search_Request_Base
    {
        public override string Dictionary => "Cancer.gov";

        public override AudienceType Audience => AudienceType.Patient;

        public override string LangCode => "en";

        public override string SearchTerm => "s-1";

        public override MatchType MatchType => MatchType.Exact;

        public override int Size => 5;

        public override int From => 0;

        public override string[] FieldList => new string[] { "termName", "definition" };

        public override JObject ExpectedRequest => JObject.Parse(@"
                {
                    ""from"": 0,
                    ""size"": 5,
                    ""_source"": {
                        ""includes"": [
                            ""term_id"",
                            ""audience"",
                            ""term_name"",
                            ""definition"",
                        ]
                    },
                    ""sort"": [
                        {
                            ""term_name"": {}
                        }
                    ],
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
                                    ""term"": {
                                        ""term_name"": {
                                            ""value"": ""s-1""
                                        }
                                    }
                                }
                            ]
                        }
                    }
                }"
            );
    }
}