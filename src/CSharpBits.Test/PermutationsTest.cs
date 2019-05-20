using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test
{
    internal static class SequenceExtensions
    {
        internal static string RemoveChar(this string s, string c)
        {
            var indexOf = s.IndexOf(c, StringComparison.CurrentCulture);
            return s.Substring(0, indexOf) + s.Substring(indexOf + 1, s.Length - indexOf - 1);
        }
    }

    public class PermutationTest
    {
        private static IEnumerable<string> Permutations(string s)
            => Permutations("", s).Select(e => e.Item1);

        private static IEnumerable<(string, string)> Permutations(string acc, string rest)
        {
            if (string.IsNullOrEmpty(rest))
                return new[] {(acc, rest)};

            return rest.SelectMany(c =>
                Permutations(
                    acc + c.ToString(),
                    rest.RemoveChar(c.ToString())
                ));
        }

        [Fact]
        void as_a_recursive_function()
        {
            var result = Permutations("abc");

            result.Should().BeEquivalentTo(new List<string>
            {
                "abc",
                "acb",
                "bac",
                "bca",
                "cab",
                "cba"
            });
        }
    }
}