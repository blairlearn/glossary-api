using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public class GetAll_Request_AdditionalFields : GetAll_Request_Base
    {
        public override string Dictionary => "Genetics";

        public override AudienceType Audience => AudienceType.HealthProfessional;

        public override string LangCode => "en";

        public override int Size => 5;

        public override int From => 200;

        public override bool IncludeAdditionalInfo => true;

        public override JObject ExpectedRequest => JObject.Parse(@"
                {
                    ""from"": 200,
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
                            ""other_languages"",
                            ""related_resources"",
                            ""media""
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
                                }
                            ]
                        }
                    }
                }"
            );
    }
}