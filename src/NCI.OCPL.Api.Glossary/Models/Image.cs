using System;

using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes an Image content item.
    /// </summary>
    public class Image : IMedia
    {
        /// <summary>
        /// Type of media this class will represent.
        /// </summary>
        /// <value>Always MediaType.Image</value>
        public MediaType Type { get; set; }

        /// <summary>
        /// Url where the image may be retrieved.
        /// </summary>
        [Keyword(Name = "ref")]
        public Uri Ref { get; set; }

        /// <summary>
        /// The image's alternate text version, suitable for displaying in an HTML alt= attribute.
        /// </summary>
        [Keyword(Name = "alt")]
        public string Alt { get; set; }

        /// <summary>
        /// String containing the image's caption.
        /// </summary>
        [Keyword(Name = "caption")]
        public string Caption { get; set; }
    }
}