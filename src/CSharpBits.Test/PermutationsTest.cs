using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CSharpBits.Test
{
    internal static class SequenceExtensions
    {
        internal static List<char> ToSequence(this string s) => s.ToList();
        internal static string ToSentence(this List<char> sequence) => string.Join("", sequence);
    }

    public class PermutationsTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly List<string> _empty = new List<string>();

        [Fact]
        void empty_string()
        {
            var result = Permutations("");

            result.Should().BeEquivalentTo(_empty);
        }

        [Fact]
        void single_char()
        {
            var result = Permutations("a");

            result.Should().BeEquivalentTo(new List<string> {"a"});
        }

        public PermutationsTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        void multiple_chars()
        {
            var result = Permutations("ab").ToList();

            foreach (var r in result)
            {
                _outputHelper.WriteLine(r);
            }

            result.OrderBy(i => i).Should().BeEquivalentTo(new List<string>
            {
                "ab",
                "ba"
            }.OrderBy(i => i));
        }
        
        [Fact]
        void generate_rests()
        {
            var result = Explode("abcde").ToList();

            result.Should().BeEquivalentTo(new List<(string, string)>
            {
                ("a", "bcde"),
                ("b", "acde"),
                ("c", "abde"),
                ("d", "abce"),
                ("e", "abcd")
            });
        }

        [Fact]
        void augment_permutations()
        {
            var result = Augment(new List<string>{"x", "y"}, "abcde").ToList();
            result.Should().BeEquivalentTo(new List<(string, string)>
            {
                ("xa", "bcde"),
                ("ya", "bcde"),
                ("xb", "acde"),
                ("yb", "acde"),
                ("xc", "abde"),
                ("yc", "abde"),
                ("xd", "abce"),
                ("yd", "abce"),
                ("xe", "abcd"),
                ("ye", "abcd")
            });
        }

        [Fact]
        void calculate_permutations()
        {
            var result = Permute("abc").ToList();
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

        private List<string> Permute(string s)
        {
            return PermuteRecursive(s, new List<string>());
        }

        private List<string> PermuteRecursive(string s, List<string> existingPermutations)
        {
            var additions = GetCharsFrom(s);

            foreach (var addition in additions)
            {
                //PermuteRecursive(s);
            }

            return existingPermutations;
        }

        private List<string> PermuteRecursive(string s, string existingPermutation)
        {
            if (string.IsNullOrEmpty(s))
                return new List<string>();

            var additions = GetCharsFrom(s);

            var permutations = new List<string>();
            foreach (var addition in additions)
            {
                permutations.AddRange(PermuteRecursive(CalculateRest(s, addition), existingPermutation + addition));
            }

            return permutations;
        }

        private List<string> AddTo(string addition, List<string> existingPermutations)
        {
            return existingPermutations.Select(p => AddToPermutation(p, addition)).ToList();
        }

        private static string AddToPermutation(string permutation, string addition) =>
            permutation + addition;

        private string CalculateRest(string s, string addition)
        {
            return s.RemoveChar(addition);
        }


        private List<string> GetCharsFrom(string s) =>
            s.Select(x => x.ToString()).ToList();

        private List<(string, string)> Augment(List<string> permutations, string s)
        {
            List<(string c, string rests)> explosions = Explode(s).ToList();


            var newPerms = new List<(string, string)>();
            foreach (var permutation in permutations)
            {
                foreach (var (c, rests) in explosions)
                {
                    newPerms.Add((permutation + c, rests));
                }
            }

            return newPerms;
        }

        [Theory]
        [InlineData("012345", 0, "12345")]
        [InlineData("012345", 1, "02345")]
        [InlineData("012345", 2, "01345")]
        [InlineData("012345", 5, "01234")]
        void remove_char_at_index(string s, int index, string expected)
        {
            s.RemoveCharAt(index).Should().Be(expected);
        }

        [Theory]
        [InlineData("012345", "0", "12345")]
        [InlineData("000000", "0", "0000")]
        [InlineData("12321", "1", "2321")]
        void remove_char(string s, string c, string expected)
        {
            s.RemoveChar(c).Should().Be(expected);
        }


        private IEnumerable<(string, string)> Explode(string s)
        {
            return s.Select((c, i) =>
            {
                return (c.ToString(), s.RemoveCharAt(i));
            });
        }



        private List<string> Permutations(string ab)
        {
            throw new System.NotImplementedException();
        }
    }

    internal static class StringExtensions
    {
        internal static string RemoveCharAt(this string s, int index) =>
            s.Substring(0, index) + s.Substring(index+1, s.Length-index-1);

        internal static string RemoveChar(this string s, string c)
        {
            var indexOf = s.IndexOf(c, StringComparison.CurrentCulture);
            return s.Substring(0, indexOf) + s.Substring(indexOf + 1, s.Length - indexOf - 1);
        }
    }
}