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
    }
}