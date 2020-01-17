using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes a single suggestion from autosuggest
    /// </summary>
    public class Suggestion
    {
        /// <summary>
        /// The term's CDR ID.
        /// </summary>
        /// <value></value>
        public long TermId { get; set; }

        /// <summary>
        /// Gets or sets the Name of the Glosary Term.
        /// </summary>
        [Keyword(Name = "term_name")]
        public string TermName { get; set; }    }
}
