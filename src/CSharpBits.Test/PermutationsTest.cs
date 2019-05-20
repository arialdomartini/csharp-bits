using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        internal static string RemoveChar(this string s, char c) =>
            s.RemoveChar(c.ToString());

        internal static string Swap(this string s, int i, int k) =>
            new StringBuilder(s)
            {
                [i] = s[k],
                [k] = s[i]
            }.ToString();
    }

    public class PermutationTest
    {
        [Theory]
        [InlineData("01234", 0, 1, "10234")]
        [InlineData("01234", 0, 4, "41230")]
        [InlineData("01234", 3, 1, "03214")]
        void swap_test(string s, int i, int j, string expected)
        {
            s.Swap(i, j).Should().Be(expected);
        }

        IEnumerable<string> PermutationsSwap(string s) =>
            PermutationsSwap(s, 0);

        private static IEnumerable<string> PermutationsSwap(string s, int from)
        {
            var permutations = new List<string> {s};

            for (var i = from; i < s.Length; i++)
            {
                for (var j = i + 1; j < s.Length; j++)
                {
                    permutations.AddRange(
                        PermutationsSwap(s.Swap(i, j), i + 1));
                }
            }

            return permutations;
        }

        [Fact]
        void with_swap()
        {
            var result = PermutationsSwap("abc");

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
            var pending = new List<(string permutation, string rest)>
            {
                ("", s)
            };
            var permutations = new List<string>();

            while (pending.Any())
            {
                pending = pending
                    .SelectMany(p =>
                        p.rest.Select(c =>
                        (
                            p.permutation + c,
                            p.rest.RemoveChar(c)
                        ))).ToList();

                permutations = pending
                    .Where(p => p.rest == "")
                    .Aggregate(permutations, (list, s1) =>
                        list.Append(s1.permutation).ToList());
            }

            return permutations;
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