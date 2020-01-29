using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for IMedia
    /// </summary>
    public class IMediaComparer : IEqualityComparer<IMedia>
    {
        public bool Equals(IMedia x, IMedia y)
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
                case MediaType.Image: {
                    var comparer = new ImageComparer();
                    return comparer.Equals((Image)x, (Image)y);
                }
                case MediaType.Video: {
                    var comparer = new VideoComparer();
                    return comparer.Equals((Video)x, (Video)y);
                }
                default:
                    throw new System.Exception("Unknown MediaType");
            }
        }

        public int GetHashCode(IMedia obj)
        {
            switch(obj.Type) {
                case MediaType.Image: {
                    var comparer = new ImageComparer();
                    return comparer.GetHashCode((Image)obj);
                }
                case MediaType.Video: {
                    var comparer = new VideoComparer();
                    return comparer.GetHashCode((Video)obj);
                }
                default:
                    throw new System.Exception("Unknown MediaType");
            }
        }

    }
}
