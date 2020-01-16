using System;

using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes an Image file source.
    /// </summary>
    public class ImageSource
    {
        /// <summary>
        /// The logical size.
        /// </summary>
        [Keyword(Name = "size")]
        public string Size { get; set; }

        /// <summary>
        /// The image's source's URI.
        /// </summary>
        [Keyword(Name = "src")]
        public Uri Src { get; set; }
    }
}