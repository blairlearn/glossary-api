using System;
using System.Security.Policy;

using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes the link to a related resource.
    /// May be Drug Information Summary, Cancer Information Summary,
    /// or an external link.
    /// </summary>
    public class LinkResource : IRelatedResource
    {
        /// <summary>
        /// Notes the related resource type.
        /// </summary>
        /// <value>One of RelatedResourceType.DrugSummary,
        /// RelatedResourceType.CancerSummary or
        /// RelatedResourceType.External
        /// </value>
        public RelatedResourceType Type { get; set; }

        /// <summary>
        /// URL of the resource item.
        /// </summary>
        public Uri Url;

        /// <summary>
        /// Short text description or name of the resource.
        /// </summary>
        [Keyword(Name = "text")]
        public string Text { get; set; }
    }
}
