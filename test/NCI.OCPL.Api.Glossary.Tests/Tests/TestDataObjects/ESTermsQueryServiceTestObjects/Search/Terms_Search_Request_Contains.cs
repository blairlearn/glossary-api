
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    class Terms_Search_Request_Contains : Terms_Search_Request_Base
    {
        public override string Dictionary => "Cancer.gov";

        public override AudienceType Audience => AudienceType.Patient;

        public override string LangCode => "es";

        public override string SearchTerm => "pollo";

        public override MatchType MatchType => MatchType.Contains;

        public override int Size => 5;

        public override int From => 0;

        public override string[] FieldList => new string[] { "termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation" };

        public override JObject ExpectedRequest => JObject.Parse(@"
                {
                    ""from"": 0,
                    ""size"": 5,
                    ""_source"": {
                        ""includes"": [
                            ""term_id"",
                            ""language"",
                            ""dictionary"",
                            ""audience"",
                            ""term_name"",
                            ""first_letter"",
                            ""pretty_url_name"",
                            ""pronunciation"",
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
                                    ""match"": {
                                        ""term_name._contain"": {
                                            ""query"": ""pollo""
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