namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Metadata about a GlossaryResults object.
    /// </summary>
    public class ResultsMetadata
    {
        /// <summary>
        /// The total number of results available.
        /// </summary>
        public int TotalResults{get;set;}

        /// <summary>
        /// Offset into the overall list where the current set begins.
        /// </summary>
        public int From {get;set;}
    }
}
