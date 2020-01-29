using System;
using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// A IEqualityComparer for GlossaryTerm
    /// </summary>
    public class GlossaryTermComparer : IEqualityComparer<GlossaryTerm>
    {
        public bool Equals(GlossaryTerm x, GlossaryTerm y)
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
                x.TermId == y.TermId
                && x.TermName == y.TermName
                && x.FirstLetter == y.FirstLetter
                && x.Language == y.Language
                && x.Dictionary.ToLower() == y.Dictionary.ToLower()
                && x.Audience.ToString() == y.Audience.ToString()
                && x.PrettyUrlName == y.PrettyUrlName
                && AreParamArraysEqual<IRelatedResource, IRelatedResourceComparer>(x.RelatedResources, y.RelatedResources)
                && AreParamArraysEqual<IMedia, IMediaComparer>(x.Media, y.Media)
                && new PronunciationComparer().Equals(x.Pronunciation, y.Pronunciation)
                && new DefinitionComparer().Equals(x.Definition, y.Definition)
            ;

            return isEqual;
            }

            public int GetHashCode(GlossaryTerm obj)
            {
            int hash = 0;
            hash ^=
                obj.TermId.GetHashCode()
                ^ obj.TermName.GetHashCode()
                ^ obj.FirstLetter.GetHashCode()
                ^ obj.Language.GetHashCode()
                ^ obj.Dictionary.GetHashCode()
                ^ obj.Audience.GetHashCode()
                ^ (obj.PrettyUrlName != null ? obj.PrettyUrlName.GetHashCode() : 0)
                ^ obj.RelatedResources.GetHashCode()
                ^ obj.Media.GetHashCode()
                ^ (obj.Pronunciation != null ? new PronunciationComparer().GetHashCode(obj.Pronunciation) : 0)
                ^ new DefinitionComparer().GetHashCode(obj.Definition)
            ;

            return hash;
        }

        /// <summary>
        /// Helper function to determine param arrays are equal, order does matter.
        /// </summary>
        /// <param name="x">Param array 1</param>
        /// <param name="y">Param array 2</param>
        /// <returns></returns>
        private bool AreParamArraysEqual<T, V>(T[] x, T[] y) where V : IEqualityComparer<T>, new()
        {
            IEqualityComparer<T> comparer = new V();

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

            if (x.Count() != y.Count())
            {
                return false;
            }

            for (int i = 0; i < x.Length; i++) {
                if (!comparer.Equals(x[i],y[i])) {
                return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Helper function to determine param arrays are equivalent, order does not matter.
        /// </summary>
        /// <param name="x">Param array 1</param>
        /// <param name="y">Param array 2</param>
        /// <returns></returns>
        private bool AreParamArraysEquiv<T, V>(T[] x, T[] y) where V : IEqualityComparer<T>, new()
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

            if (x.Count() != y.Count())
            {
                return false;
            }

            //Generate a set of those values that are not in both lists.
            //if this is not 0, then there is an error.
            var diffxy = x.Except(y, new V());

            return diffxy.Count() == 0;
        }
    }
}