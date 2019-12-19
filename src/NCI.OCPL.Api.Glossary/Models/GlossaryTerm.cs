using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// The GlossaryTerm class
    /// </summary>
    [ElasticsearchType(Name = "terms")]
    public class GlossaryTerm
    {

        /// <summary>
        /// Gets or sets the Id for the Glosary Term
        /// </summary>
        [Number(NumberType.Integer, Name = "term_id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Language for the Glosary Term
        /// </summary>
        [Keyword(Name = "language")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the Dictionary for the Glosary Term
        /// </summary>
        [Keyword(Name = "dictionary")]
        public string Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the AudienceType for the Glosary Term
        /// </summary>
        [Nested(Name = "audience")]
        public AudienceType Audience { get; set; }

        /// <summary>
        /// Gets or sets the TermName for the Glosary Term
        /// </summary>
        [Keyword(Name = "term_name")]
        public string TermName { get; set; }

        /// <summary>
        /// Gets or sets the prettyUrlName for the Glosary Term
        /// </summary>
        [Keyword(Name = "pretty_url_name")]
        public string  PrettyUrlName { get; set; }

        /// <summary>
        /// Gets or sets the pronounciation for the Glosary Term
        /// </summary>
        [Nested(Name = "pronounciation")]
        public Pronounciation Pronounciation  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary>
        [Nested(Name = "definition")]
        public Definition Definition  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary>
        [Nested(Name = "related_resources")]
        public IRelatedResource[] RelatedResources  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary>
        [Nested(Name = "media")]
        public IMedia[] Media  { get; set; }

        /// <summary>
        /// no arg constructor
        /// </summary>
        public GlossaryTerm() {}
    }
}
