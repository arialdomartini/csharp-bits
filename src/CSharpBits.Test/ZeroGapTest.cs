using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test
{
    internal static class BitsExtensions
    {
        internal static List<int> ToBits(this string s) =>
            s.Select(c => int.Parse(c.ToString())).ToList();

        internal static int Head(this IEnumerable<int> bits) =>
            bits.First();

        internal static List<int> Tail(this IEnumerable<int> bits) =>
            bits.Skip(1).ToList();

        internal static bool IsSeparator(this int i) => i == 1;
        internal static bool IsZero(this int i) => !i.IsSeparator();
        internal static bool IsEmpty(this IEnumerable<int> bits) => !bits.Any();
    }

    public class ZeroGapTest
    {
        private static readonly List<int> Empty = new List<int>();

        [Fact]
        void empty_case()
        {
            Gap(Empty).Should().Be(0);
        }

        [Fact]
        void one_sequence()
        {
            Gap("10001".ToBits()).Should().Be(3);
        }

        [Theory]
        [InlineData("10001110000011", 5)]
        [InlineData("1000111011", 3)]
        void more_sequences(string s, int expected)
        {
            Gap(s.ToBits()).Should().Be(expected);
        }

        [Theory]
        [InlineData("100011100000110000000000000000000", 5)]
        [InlineData("00000001000111011", 3)]
        void ignore_unbounded_sequences(string s, int expected)
        {
            Gap(s.ToBits()).Should().Be(expected);
        }

        private int Gap(List<int> bits) =>
            GapRecursive(bits, 0, 0, false);

        private int GapRecursive(List<int> bits, int longest, int current, bool started)
        {
            if (bits.IsEmpty())
                return longest;

            var head = bits.Head();
            var tail = bits.Tail();

            return GapRecursive(
                tail,
                SequenceHasEnded(started, head) ? Math.Max(current, longest) : longest,
                CanIncrement(started, head) ? current + 1 : 0,
                SequenceHasStarted(started, head));

        }

        private static bool SequenceHasEnded(bool started, int head) =>
            head.IsSeparator() && started;

        private static bool CanIncrement(bool started, int head) =>
            head.IsZero() && started;

        private static bool SequenceHasStarted(bool started, int head) =>
            head.IsSeparator() || started;
    }
}