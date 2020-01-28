using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// An IEqualityComparer for Suggestion objects.
    /// </summary>
    public class SuggestionComparer : IEqualityComparer<Suggestion>
    {
        public bool Equals(Suggestion x, Suggestion y)
        {
            // If the items are both null, or if one or the other is null, return
            // the correct response right away.
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            bool isEqual =
                x.TermId == y.TermId &&
                x.TermName == y.TermName
            ;

            return isEqual;
        }

        public int GetHashCode(Suggestion obj)
        {
            int hash = 0;

            hash ^=
                obj.TermId.GetHashCode()
                ^ obj.TermName.GetHashCode()
            ;

            return hash;
        }
    }
}