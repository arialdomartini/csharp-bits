using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test
{
    public class RecursionUnroll
    {
        [Fact]
        void sum_recursive()
        {
            var result = Sum(10);

            result.Should().Be(0 + 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10);
        }

        [Fact]
        void sum_ints_in_a_list()
        {
            var ints = new List<int> {1, 2, 3};

            Sum(ints).Should().Be(1 + 2 + 3);
        }

        private int Sum(List<int> ints)
        {
            return Unroll(
                ints,
                0,
                list => !list.Any(),
                (sum, list) => sum + list.First(),
                list => list.Skip(1).ToList()
            );
        }

        private int Sum(int n)
        {
            return Unroll(
                input: n,
                initialValue: 0,
                termination: v => v == 0,
                operation: (acc, v) => acc + v,
                step: v => v - 1);
        }

        private static TOut Unroll<TIn, TOut>(
            TIn input,
            TOut initialValue,
            Func<TIn, bool> termination,
            Func<TOut, TIn, TOut> operation,
            Func<TIn, TIn> step)
        {
            while (true)
            {
                if (termination(input)) return initialValue;

                initialValue = operation(initialValue, input);
                input = step(input);
            }
        }
    }
}