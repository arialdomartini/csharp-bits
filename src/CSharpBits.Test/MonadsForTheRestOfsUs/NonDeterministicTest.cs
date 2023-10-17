using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using LanguageExt;
using Xunit;
using static CSharpBits.Test.MonadsForTheRestOfsUs.Nondeterministic.NonDeterministicExtensions;

namespace CSharpBits.Test.MonadsForTheRestOfsUs.Nondeterministic;

using Position = ValueTuple<int, int>;

//record Position(int X, int Y);

internal static class NonDeterministicExtensions
{
    internal static Nond<B> Bind<A, B>(this Func<A, Nond<B>> f, Nond<A> m)
    {
        IEnumerable<A> values = m.Run();
        IEnumerable<Nond<B>> mappedNested = values.Select(f);
        IEnumerable<B> flat = mappedNested.SelectMany(m => m.Run());
        return new Nond<B>(flat.ToArray());
    }

    internal static Func<A, Nond<C>> Compose<A, B, C>(Func<B, Nond<C>> f, Func<A, Nond<B>> g) => 
        a => f.Bind(g(a));

    internal static Nond<A> Return<A>(this A a) => new Nond<A>(new[] { a });
}

public class NonDeterministicTest
{
    [Fact]
    void test_for_bind()
    {
        Nond<int> PreviousFunction() =>
            new Nond<int>(new[]
            {
                1,
                2
            });

        Func<int, Nond<(int, char)>> g = j =>
            new Nond<(int, char)>(new[]
            {
                (j, 'a'),
                (j, 'b')
            });

        Nond<int> nondValue = PreviousFunction();

        Nond<(int, char)> bind = g.Bind(nondValue);

        IEnumerable<(int, char)> combinations = bind.Run();
        Assert.Equal(new[] { (1,'a'), (1, 'b'), (2, 'a'), (2, 'b') }, combinations);
    }

    [Fact]
    void return_for_nondet()
    {
        var position = (5, 5);
        var monadicPosition = position.Return();

        var allCombinations = monadicPosition.Run();

        Assert.Equal(1, allCombinations.Length());
        Assert.Equal(new[] { position }, allCombinations);
    }

    [Fact]
    void move_a_knight()
    {
        Func<Position, Nond<Position>> move = p =>
        {
            var (x, y) = p;
            var positions = new[]
            {
                (x + 1, y + 2),
                (x - 1, y + 2),
                (x + 1, y - 2),
                (x - 1, y - 2),
                (x + 2, y + 1),
                (x - 2, y + 1),
                (x + 2, y - 1),
                (x - 2, y - 1)
            };

            return new Nond<(int, int)>(positions);
        };

        Position start = (5, 5);
        Nond<Position> step1 = move(start);
        Nond<Position> step2 = move.Bind(step1);
        Nond<Position> step3 = move.Bind(step2);

        IEnumerable<(int, int)> allPossiblePositions = step3.Run();
        Assert.Equal(512, allPossiblePositions.Length());

        //
        // var stepsLinq = from s1 in move(start)
        //     from s2 in move(s1)
        //     from s3 in move(s2)
        //     select s3;


        // var startingPoint = (5, 5).Return();
        // var steps =
        //     Enumerable.Range(1, 10)
        //         .Aggregate(startingPoint, (current, _) => move.Bind(current))
        //         .Run();
        //
        // Assert.Equal(1073741824, steps.Length());
        // Assert.Equal(604, steps.Distinct().Length());
    }

    [Fact]
    void compose()
    {
        Func<Position, Nond<Position>> move = p =>
        {
            var (x, y) = p;
            var positions = new[]
            {
                (x + 1, y + 2),
                (x - 1, y + 2),
                (x + 1, y - 2),
                (x - 1, y - 2),
                (x + 2, y + 1),
                (x - 2, y + 1),
                (x + 2, y - 1),
                (x - 2, y - 1)
            };

            return new Nond<(int, int)>(positions);
        };


        Func<Position,Nond<Position>> composed = Compose(move, move);

        Position start = (5, 5);
        IEnumerable<Position> allPossiblePositions = composed(start).Run();

        Assert.Equal(64, allPossiblePositions.Length());
    }
}
