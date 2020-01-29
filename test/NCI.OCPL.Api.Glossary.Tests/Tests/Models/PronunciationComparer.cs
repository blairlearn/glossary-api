using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for Pronunciation
    /// </summary>
    public class PronunciationComparer : IEqualityComparer<Pronunciation>
    {
        public bool Equals(Pronunciation x, Pronunciation y)
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
                x.Audio == y.Audio
                && x.Key == y.Key
            ;

            return isEqual;
            }

            public int GetHashCode(Pronunciation obj)
            {
            int hash = 0;
            hash ^=
                (obj.Audio != null ? obj.Audio.GetHashCode() : 0)
                ^ (obj.Key != null ? obj.Key.GetHashCode() : 0)
            ;

            return hash;
        }
    }
}

