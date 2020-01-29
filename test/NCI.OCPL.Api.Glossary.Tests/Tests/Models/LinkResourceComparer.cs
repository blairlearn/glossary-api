using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for LinkResource
    /// </summary>
    public class LinkResourceComparer : IEqualityComparer<LinkResource>
    {
        public bool Equals(LinkResource x, LinkResource y)
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
                x.Url.Equals(y.Url)
                && x.Text == y.Text
                && x.Type == y.Type;

            return isEqual;
            }

            public int GetHashCode(LinkResource obj)
            {
            int hash = 0;
            hash ^=
                obj.Url.GetHashCode()
                ^ obj.Text.GetHashCode()
                ^ obj.Type.GetHashCode();

            return hash;
        }
    }
}

