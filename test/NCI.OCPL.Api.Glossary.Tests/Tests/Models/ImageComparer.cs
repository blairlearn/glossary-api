using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests
{
  /// <summary>
  /// A IEqualityComparer for Image
  /// </summary>
  public class ImageComparer : IEqualityComparer<Image>
  {
    public bool Equals(Image x, Image y)
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
        x.Type == y.Type
        && x.Ref == y.Ref
        && x.Caption == y.Caption
        && x.Alt == y.Alt
      ;

      return isEqual;
    }

    public int GetHashCode(Image obj)
    {
      int hash = 0;
      hash ^=
        obj.Type.GetHashCode()
        ^ (obj.Ref != null ? obj.Ref.GetHashCode() : 0)
        ^ (obj.Caption != null ? obj.Caption.GetHashCode() : 0)
        ^ (obj.Alt != null ? obj.Alt.GetHashCode() : 0)
      ;

      return hash;
    }
  }
}

