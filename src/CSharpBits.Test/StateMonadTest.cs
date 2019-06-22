using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static CSharpBits.Test.StateMonadTest;

namespace CSharpBits.Test
{

    internal static class GeneratorExtensions
    {
        internal static TValue Run<TValue>(this Generator<TValue> generator, int seed) =>
            generator(seed).Value;

        internal static TValue Run<TValue>(this Generator<TValue> generator)
            => generator.Run(Environment.TickCount);

        internal static Generator<TB> Map<TA, TB>(
            this Generator<TA> generator,
            Func<TA, TB> func) =>

            seed =>
            {
                var (randomA, newSeed) = generator(seed);
                return (func(randomA), newSeed);
            };
    }

    public class StateMonadTest
    {
        private readonly ITestOutputHelper _outputHelper;

        // TValue -> (TValue, int)
        internal delegate (TValue Value, int Seed) Generator<TValue>(int seed);

        [Fact]
        public void should_generate_random_numbers()
        {
            var (random1, seed1) = NextInt(0);
            var (random2, seed2) = NextInt(seed1);
            var (random3, _) = NextInt(seed2);

            random1.Should().Be(100);
            random2.Should().Be(-21);
            random3.Should().Be(50);
        }

        private (int Value, int Seed) NextInt(int seed)
        {
            if (seed == 0) return (100, 1);
            if (seed == 1) return (-21, 2);
            if (seed == 2) return (50, 3);
            throw new Exception("Sequence ended");
        }
        
        [Fact]
        public void should_generate_other_primitives()
        {
            (bool random, int newSeed) NextBool(int seed)
            {
                var (randomInt, newSeed) = NextInt(seed);
                return (randomInt % 2 == 0, newSeed);
            }

            var (random1, seed1) = NextBool(0);
            var (random2, seed2) = NextBool(seed1);
            var (random3, _) = NextBool(seed2);

            random1.Should().Be(true);
            random2.Should().Be(false);
            random3.Should().Be(true);
        }

        [Fact]
        public void should_generate_other_primitives_using_Map()
        {
            Generator<int> nextInt = NextInt;
            Generator<bool> nextBool = nextInt.Map(i =>
            {
                return i % 2 == 0;
            });

            var (random1, seed1) = nextBool(0);
            var (random2, seed2) = nextBool(seed1);
            var (random3, _) = nextBool(seed2);

            random1.Should().Be(true);
            random2.Should().Be(false);
            random3.Should().Be(true);
        }
    }
}