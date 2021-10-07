using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// The GlossaryTerm class
    /// </summary>
    public class GlossaryTerm
    {

        /// <summary>
        /// Gets or sets the Id for the Glosary Term
        /// </summary>
        [Number(NumberType.Long, Name = "term_id")]
        public long TermId { get; set; }

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
        [JsonConverter(typeof(StringEnumConverter))]
        public AudienceType Audience { get; set; }

        /// <summary>
        /// Gets or sets the TermName for the Glosary Term
        /// </summary>
        [Keyword(Name = "term_name")]
        public string TermName { get; set; }

        /// <summary>
        /// Gets or sets the FirstLetter for the Glosary Term
        /// </summary>
        [Keyword(Name = "first_letter")]
        public string FirstLetter { get; set; }

        /// <summary>
        /// Gets or sets the prettyUrlName for the Glosary Term
        /// </summary>
        [Keyword(Name = "pretty_url_name")]
        public string  PrettyUrlName { get; set; }

        /// <summary>
        /// Gets or sets the pronunciation for the Glosary Term
        /// </summary>
        [Nested(Name = "pronunciation")]
        public Pronunciation Pronunciation  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary>
        [Nested(Name = "definition")]
        public Definition Definition  { get; set; }

        /// <summary>
        /// Gets or sets the translations of this term.
        /// </summary>
        [Nested(Name = "other_languages")]
        public TermOtherLanguage[] OtherLanguages { get; set; } = new TermOtherLanguage[] { };

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary>
        [Nested(Name = "related_resources")]
        [JsonProperty(ItemConverterType = typeof(RelatedResourceJsonConverter))]
        public IRelatedResource[] RelatedResources  { get; set; } = new IRelatedResource[] { };

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary>
        [Nested(Name = "media")]
        [JsonProperty(ItemConverterType = typeof(MediaJsonConverter))]
        public IMedia[] Media  { get; set; } = new IMedia[] { };

        /// <summary>
        /// no arg constructor
        /// </summary>
        public GlossaryTerm() {}
    }
}
