using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for Definition
    /// </summary>
    public class DefinitionComparer : IEqualityComparer<Definition>
    {
        public bool Equals(Definition x, Definition y)
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
                x.Html == y.Html
                && x.Text == y.Text
            ;

            return isEqual;
            }

            public int GetHashCode(Definition obj)
            {
            int hash = 0;
            hash ^=
                obj.Html.GetHashCode()
                ^ obj.Text.GetHashCode()
            ;

            return hash;
        }
    }
}

