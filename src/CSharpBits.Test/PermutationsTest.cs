using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test
{
    internal static class SequenceExtensions
    {
        internal static List<char> ToSequence(this string s) => s.ToList();
        internal static string Head(this List<string> list) => list.FirstOrDefault();

        internal static List<string> RemoveItem(this List<string> list, string item) =>
            list.Where((e, i) => i != list.IndexOf(item)).ToList();
        internal static List<string> Tail(this List<string> list) => list.Skip(1).ToList();
        internal static string ToSentence(this List<string> s) => s.Aggregate("", (a, i) => a + i);
        internal static string ToSentence(this List<char> sequence) => string.Join("", sequence);
        internal static string RemoveCharAt(this string s, int index) =>
            s.Substring(0, index) + s.Substring(index+1, s.Length-index-1);

        internal static string RemoveChar(this string s, string c)
        {
            var indexOf = s.IndexOf(c, StringComparison.CurrentCulture);
            return s.Substring(0, indexOf) + s.Substring(indexOf + 1, s.Length - indexOf - 1);
        }
    }

    public class PermutationsTest
    {
        [Fact]
        void remove_item()
        {
            var list = new List<string>{"a", "b", "c"};
            list.RemoveItem("b").Should().BeEquivalentTo(new List<string> {"a", "c"});
        }

        [Fact]
        void to_sentence()
        {
            var list = new List<string>{"a", "b", "c"};

            list.ToSentence().Should().Be("abc");
        }

        [Fact]
        void explode_from_empty()
        {
            var result = Explode(new List<string>(), "abc");

            result.Should().BeEquivalentTo(new List<string> {"a", "b", "c"});
        }

        [Fact]
        void explode_from_one()
        {
            var result = Explode(new List<string>{"a"}, "bc");

            result.Should().BeEquivalentTo(new List<string> {"ab", "ac"});
        }

        [Fact]
        void explode_from_two()
        {
            var result = Explode(new List<string>{"x", "y"}, "bc");

            result.Should().BeEquivalentTo(new List<string> {"xb", "xc", "yb", "yc"});
        }

        [Fact]
        void explode_rest_from_two()
        {
            var result = ExplodeRest(new List<string>{"x", "y"}, "bc");

            result.Should().BeEquivalentTo(new List<(string, string)>
            {
                ("xb", "c"),
                ("xc", "b"),
                ("yb", "c"),
                ("yc", "b")
            });
        }

        private IEnumerable<string> Explode(List<string> list, string s)
        {
            if(!list.Any())
                return s.Select(c => c.ToString());


            return list.Aggregate(
                new List<string>(),
                (e, c) => e.Union(
                    Extend(c, s)).ToList());
        }

        private IEnumerable<(string, string)> ExplodeRest(List<string> list, string s)
        {
            if(!list.Any())
                return s.Select(c => (c.ToString(), ""));


            return list.Aggregate(
                new List<(string, string)>(),
                (e, c) => e.Union(
                    ExtendRest(c, s)).ToList());
        }

        private static IEnumerable<string> Extend(string element, string s) =>
            s.Select(c => element + c);

        private static IEnumerable<(string, string)> ExtendRest(string element, string s) =>
            s.Select(c => (element + c, s.RemoveChar(c.ToString())));
    }
}