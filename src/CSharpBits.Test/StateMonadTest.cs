using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static CSharpBits.Test.StateMonadTest;

namespace CSharpBits.Test;

internal static class GeneratorExtensions
{
    private static TValue Run<TValue>(this Generator<TValue> generator, int seed) =>
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

    internal static Generator<TB> Select<TA, TB>(
        this Generator<TA> generator,
        Func<TA, TB> func)
    {
        return Map(generator, func);
    }

    internal static Generator<TB> Bind<TA, TB>(
        this Generator<TA> generator,
        Func<TA, Generator<TB>> f
    )
    {
        return seed =>
        {
            var (random, newSeed) = generator(seed);
            var newGenerator = f(random);
            return newGenerator(newSeed);
        };
    }

    internal static Generator<TC> SelectMany<TA, TB, TC>(
        this Generator<TA> generator,
        Func<TA, Generator<TB>> bind,
        Func<TA, TB, TC> project)
    {
        return seed =>
        {
            (TA random, int seed1) = generator(seed);
            (TB random2, int seed2) = bind(random)(seed1);
            TC state = project(random, random2);
            return (state, seed2);
        };
    }
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
        if (seed == 3) return (99, 4);
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
        Generator<bool> nextBool = nextInt.Map(i => i % 2 == 0);

        var (random1, seed1) = nextBool(0);
        var (random2, seed2) = nextBool(seed1);
        var (random3, _) = nextBool(seed2);

        random1.Should().Be(true);
        random2.Should().Be(false);
        random3.Should().Be(true);
    }


    [Fact]
    public void should_generate_other_primitives_using_Linq()
    {
        Generator<int> nextInt = NextInt;
        Generator<bool> nextBool =
            from i in nextInt
            select i % 2 == 0;

        var (random1, seed1) = nextBool(0);
        var (random2, seed2) = nextBool(seed1);
        var (random3, _) = nextBool(seed2);

        random1.Should().Be(true);
        random2.Should().Be(false);
        random3.Should().Be(true);
    }

    [Fact]
    public void should_generate_complex_types_with_linq()
    {
        Generator<int> nextInt = NextInt;

        var nextTuple =
            from a in nextInt
            from b in nextInt
            from c in nextInt
            from d in nextInt
            select (a: a, b.ToString(), c, d.ToString());

        var nextTuple2 =
            nextInt
                .SelectMany(a => nextInt, (a, b) => new {a, b})
                .SelectMany(t => nextInt, (prev, c) => new {prev, c})
                .SelectMany(t => nextInt, (prev, d) =>
                    (
                        a: prev.prev.a,
                        prev.prev.b.ToString(),
                        prev.c,
                        d.ToString())
                );

        var v1 = nextTuple(0);

        v1.Value.Should().Be((100, "-21", 50, "99"));
    }
}