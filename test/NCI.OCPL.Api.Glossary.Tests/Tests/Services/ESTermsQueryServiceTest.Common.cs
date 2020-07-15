using System.IO;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using NCI.OCPL.Api.Glossary.Models;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public partial class ESTermsQueryServiceTest
    {

        ///<summary>
        ///A private method to enrich IOptions
        ///</summary>
        private IOptions<GlossaryAPIOptions> GetMockOptions()
        {
            Mock<IOptions<GlossaryAPIOptions>> glossaryAPIClientOptions = new Mock<IOptions<GlossaryAPIOptions>>();
            glossaryAPIClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new GlossaryAPIOptions()
                {
                    AliasName = "glossaryv1"
                }
            );

            return glossaryAPIClientOptions.Object;
        }

        /// <summary>
        /// Simulates a "no results found" response from Elasticsearch so we
        /// have something for tests where we don't care about the response.
        /// </summary>
        private Stream MockEmptyResponse
        {
            get
            {
                string empty = @"
{
    ""took"": 223,
    ""timed_out"": false,
    ""_shards"": {
        ""total"": 1,
        ""successful"": 1,
        ""skipped"": 0,
        ""failed"": 0
    },
    ""hits"": {
        ""total"": 0,
        ""max_score"": null,
        ""hits"": []
    }
}";
                byte[] byteArray = Encoding.UTF8.GetBytes(empty);
                return new MemoryStream(byteArray);
            }
        }    }
}