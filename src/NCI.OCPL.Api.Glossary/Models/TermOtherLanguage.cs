using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Represets a glossary term in another language
    /// </summary>
    public class TermOtherLanguage
    {
        /// <summary>
        /// Gets or sets the Language for the Glosary Term
        /// </summary>
        [Keyword(Name = "language")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the TermName for the translation
        /// </summary>
        [Keyword(Name = "term_name")]

        public string TermName { get; set; }
        /// <summary>
        /// If available, the translation's human readable name, rendered in a URL-friendly format.
        /// </summary>
        /// <value>Empty string if no human-readable name is available.</value>
        [Keyword(Name = "pretty_url_name")]
        public string PrettyUrlName{ get; set; }
    }
}