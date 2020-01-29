using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for IRelatedResource
    /// </summary>
    public class IRelatedResourceComparer : IEqualityComparer<IRelatedResource>
    {
        public bool Equals(IRelatedResource x, IRelatedResource y)
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
            else if (x.Type != y.Type) // Not the same type of resource
            {
                return false;
            }

            switch(x.Type) {
                // Link Resource
                case RelatedResourceType.DrugSummary:
                case RelatedResourceType.Summary:
                case RelatedResourceType.External: {
                    var comparerLR = new LinkResourceComparer();
                    return comparerLR.Equals((LinkResource)x, (LinkResource)y);
                }
                // Glossary Resource
                case RelatedResourceType.GlossaryTerm: {
                    var comparerGR = new GlossaryResourceComparer();
                    return comparerGR.Equals((GlossaryResource)x, (GlossaryResource)y);
                }
                default:
                    throw new System.Exception("Unknown RelatedResourceType");
            }
        }

        public int GetHashCode(IRelatedResource obj)
        {
            switch(obj.Type) {
                // Link Resource
                case RelatedResourceType.DrugSummary:
                case RelatedResourceType.Summary:
                case RelatedResourceType.External: {
                    var comparerLR = new LinkResourceComparer();
                    return comparerLR.GetHashCode((LinkResource)obj);
                }
                // Glossary Resource
                case RelatedResourceType.GlossaryTerm: {
                    var comparerGR = new GlossaryResourceComparer();
                    return comparerGR.GetHashCode((GlossaryResource)obj);
                }
                default:
                    throw new System.Exception("Unknown RelatedResourceType");
            }
        }

    }
}
