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
        public RelatedResourceType Type { get; set; }

        /// <summary>
        /// Short text description or name of the resource.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The glossary term's CDR ID.
        /// </summary>
        /// <value></value>
        public long Id {get; set;}

        /// <summary>
        /// The name of the dictionary the GlossaryTerm belongs to.
        /// </summary>
        /// <value>
        /// "Cancer.gov" for the Dictionary of Cancer Terms.
        /// "genetic" for the Dictionary of Genetics Terms.
        /// </value>
        public string Dictionary{ get; set; }

        /// <summary>
        /// The GlossaryTerm's intended audience.
        /// </summary>
        /// <value>
        /// healthprofessional - Doctors and other health professionals.
        /// patient - Patients, friends, and family members.
        /// </value>
        public string Audience{ get; set; }

        /// <summary>
        /// If available, the term's human readable name, rendered in a URL-friendly format.
        /// </summary>
        /// <value>Empty string if no human-readable name is available.</value>
        public string PrettyUrlName{ get; set; }
    }
}