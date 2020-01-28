using System.Collections.Generic;
using Xunit;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Make sure that the test helper array comparison class works as specified:
    /// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1.gethashcode
    /// </summary>
    public class ArrayComparerTest
    {
        /// <summary>
        /// The array the others will be compared to.
        /// </summary>
        static public Suggestion[] baseline = new Suggestion[]
        {
                new Suggestion
                {
                    TermId = 123,
                    TermName = "chicken"
                },
                new Suggestion
                {
                    TermId = 456,
                    TermName = "not a chicken"
                },
                new Suggestion
                {
                    TermId = 789,
                    TermName = "turducken"
                }
        };

        /// <summary>
        /// An array identical to the baseline.
        /// </summary>
        static public Suggestion[] matching = new Suggestion[]
        {
                new Suggestion
                {
                    TermId = 123,
                    TermName = "chicken"
                },
                new Suggestion
                {
                    TermId = 456,
                    TermName = "not a chicken"
                },
                new Suggestion
                {
                    TermId = 789,
                    TermName = "turducken"
                }
        };

        /// <summary>
        /// An array of suggestions which don't match the baseline.
        /// </summary>
        static public Suggestion[] differing = new Suggestion[]
        {
                new Suggestion
                {
                    TermId = 321,
                    TermName = "chicken"
                },
                new Suggestion
                {
                    TermId = 456,
                    TermName = "frog"
                },
                new Suggestion
                {
                    TermId = 789,
                    TermName = "turducken"
                }
        };

        /// <summary>
        /// An array of suggestions shorter than the baseline
        /// </summary>
        static public Suggestion[] fewer = new Suggestion[]
        {
                new Suggestion
                {
                    TermId = 123,
                    TermName = "chicken"
                },
                new Suggestion
                {
                    TermId = 456,
                    TermName = "not a chicken"
                }
        };

        /// <summary>
        /// An array of suggestions longer than the baseline
        /// </summary>
        static public Suggestion[] more = new Suggestion[]
        {
                new Suggestion
                {
                    TermId = 123,
                    TermName = "chicken"
                },
                new Suggestion
                {
                    TermId = 456,
                    TermName = "not a chicken"
                },
                new Suggestion
                {
                    TermId = 789,
                    TermName = "turducken"
                },
                new Suggestion
                {
                    TermId = 999,
                    TermName = "quail"
                }
        };

        /// <summary>
        /// An empty array of suggestions.
        /// </summary>
        static public Suggestion[] none = new Suggestion[] { };

        public static IEnumerable<object[]> MismatchArrayData = new[]
        {
            new object[] { differing },
            new object[] { fewer },
            new object[] { more },
            new object[] { none }
        };

        /// <summary>
        /// Test that Equals returns true for identical arrays.
        /// </summary>
        [Fact]
        public void Equals_True_For_Identical()
        {
            ArrayComparer<Suggestion, SuggestionComparer> comparer = new ArrayComparer<Suggestion, SuggestionComparer>();
            Assert.True(comparer.Equals(baseline, matching));
        }

        /// <summary>
        /// Test that Equals returns false for mismatched arrays.
        /// </summary>
        [Theory, MemberData(nameof(MismatchArrayData))]
        public void Equals_False_For_Mismatches(Suggestion[] data)
        {
            ArrayComparer<Suggestion, SuggestionComparer> comparer = new ArrayComparer<Suggestion, SuggestionComparer>();
            Assert.False(comparer.Equals(baseline, data));
        }

        /// <summary>
        ///  Verify that GetHashCode() returns the same value for arrays which are equal.
        /// </summary>
        [Fact]
        public void GetHashCode_Same_For_Matches()
        {
            // NOTE: Equals_True_For_Identical() verifies that baseline and matching return equal.
            // The spec says GetHashCode() should therefore return the same value.
            ArrayComparer<Suggestion, SuggestionComparer> comparer = new ArrayComparer<Suggestion, SuggestionComparer>();
            Assert.Equal(comparer.GetHashCode(baseline), comparer.GetHashCode(matching));
        }

        /// <summary>
        /// Verify GetHashCode() returns different values for arrays which aren't the same.
        /// </summary>
        [Theory, MemberData(nameof(MismatchArrayData))]
        public void GetHashCode_Different_For_Mismatches(Suggestion[] data)
        {
            ArrayComparer<Suggestion, SuggestionComparer> comparer = new ArrayComparer<Suggestion, SuggestionComparer>();
            Assert.NotEqual(comparer.GetHashCode(baseline), comparer.GetHashCode(data));
        }
    }
}