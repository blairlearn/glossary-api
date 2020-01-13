using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes a related GlossaryTerm.
    /// </summary>
    public class GlossaryResource : IRelatedResource
    {
        /// <summary>
        /// Notes the related resource type.
        /// </summary>
        /// <value>Always RelatedResourceType.GlossaryTerm</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public RelatedResourceType Type { get; set; }

        /// <summary>
        /// Short text description or name of the resource.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The glossary term's CDR ID.
        /// </summary>
        /// <value></value>
        [Number(NumberType.Long, Name = "term_id")]
        public long TermId {get; set;}

        /// <summary>
        /// The GlossaryTerm's intended audience.
        /// </summary>
        /// <value>
        /// healthprofessional - Doctors and other health professionals.
        /// patient - Patients, friends, and family members.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public AudienceType Audience{ get; set; }

        /// <summary>
        /// If available, the term's human readable name, rendered in a URL-friendly format.
        /// </summary>
        /// <value>Empty string if no human-readable name is available.</value>
        [Keyword(Name = "pretty_url_name")]
        public string PrettyUrlName{ get; set; }
    }
}