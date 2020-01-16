using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests
{
  /// <summary>
  /// A IEqualityComparer for ImageSource
  /// </summary>
  public class ImageSourceComparer : IEqualityComparer<ImageSource>
  {
    public bool Equals(ImageSource x, ImageSource y)
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
        x.Size == y.Size
        && (x.Src != null ? x.Src.Equals(y.Src) : x.Src == y.Src)
      ;

      return isEqual;
    }

    public int GetHashCode(ImageSource obj)
    {
      int hash = 0;
      hash ^=
        obj.Size.GetHashCode()
        ^ obj.Src.GetHashCode()
      ;

      return hash;
    }
  }
}

