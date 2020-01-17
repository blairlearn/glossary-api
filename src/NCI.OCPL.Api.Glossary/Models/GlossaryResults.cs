
namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Represents the results of a glossary search operation.
    /// </summary>
    public class GlossaryTermResults
    {
        /// <summary>
        /// Metadata about the results.
        /// </summary>
        public ResultsMetadata Meta;

        /// <summary>
        /// Array of GlossaryTerm objects matching the search. May be empty.
        /// </summary>
        public GlossaryTerm[] Results;

        /// <summary>
        /// Link to ????
        /// </summary>
        public Metalink Links;
    }
}