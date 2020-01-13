using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests
{
  /// <summary>
  /// A IEqualityComparer for Video
  /// </summary>
  public class VideoComparer : IEqualityComparer<Video>
  {
    public bool Equals(Video x, Video y)
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
        && x.UniqueId == y.UniqueId
        && x.Title == y.Title
        && x.Hosting == y.Hosting
        && x.Caption == y.Caption
        && x.Template == y.Template
      ;

      return isEqual;
    }

    public int GetHashCode(Video obj)
    {
      int hash = 0;
      hash ^=
        obj.Type.GetHashCode()
        ^ obj.Ref.GetHashCode()
        ^ obj.UniqueId.GetHashCode()
        ^ obj.Hosting.GetHashCode()
        ^ (obj.Caption != null ? obj.Caption.GetHashCode() : 0)
        ^ (obj.Template != null ? obj.Template.GetHashCode() : 0)
      ;

      return hash;
    }
  }
}

