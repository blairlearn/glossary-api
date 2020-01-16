using System;

using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonConverter(typeof(StringEnumConverter))]
        public MediaType Type { get; set; }

        /// <summary>
        /// A collection of image source files.
        /// </summary>
        /// <value></value>
        [Nested(Name = "image_sources")]
        public ImageSource[] ImageSources { get; set; }

        /// <summary>
        /// The CDR ID of the referenced image.
        /// </summary>
        [Keyword(Name = "ref")]
        public string Ref { get; set; }

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