using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for TermOtherLanguage
    /// </summary>
    public class TermOtherLanguageComparer : IEqualityComparer<TermOtherLanguage>
    {
        public bool Equals(TermOtherLanguage x, TermOtherLanguage y)
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
                x.Language == y.Language
                && x.TermName == y.TermName
                && x.PrettyUrlName == y.PrettyUrlName;

            return isEqual;
            }

            public int GetHashCode(TermOtherLanguage obj)
            {
            int hash = 0;
            hash ^=
                obj.Language.GetHashCode()
                ^ obj.TermName.GetHashCode()
                ^ obj.PrettyUrlName.GetHashCode();

            return hash;
        }
    }
}

