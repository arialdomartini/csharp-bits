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

        internal static string RemoveChar(this string s, char c)
        {
            var indexOf = s.IndexOf(c.ToString(), StringComparison.CurrentCulture);
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

        private static IEnumerable<string> PermutationsIterative(string s)
        {
            var permutations = new List<(string permutation, string rest)>
            {
                ("", s)
            };

            while (true)
            {
                if (permutations.All(p => p.rest == ""))
                    return permutations.Select(p => p.permutation);

                permutations = permutations.SelectMany(p =>
                    p.rest.Select(c =>
                    (
                        p.permutation + c,
                        p.rest.RemoveChar(c)
                    ))).ToList();
            }
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

        [Fact]
        void as_an_iterative_function()
        {
            var result = PermutationsIterative("abc");

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