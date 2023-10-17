using System;
using Xunit;
using static Functional.Simplified.Extensions;

namespace Functional.Simplified;

public class FunctionalProgramming
{
    [Fact]
    internal void check_functional_composition()
    {
        var foo = "foo";

        var square = Compose(Square, Length);

        Assert.Equal(9, square(foo));
    }

    [Fact]
    internal void check_duplicate_string()
    {
        var foo = "foo";

        var square = Compose(Length, Duplicate);
        Assert.Equal(6, square(foo));
    }

    [Fact]
    internal void check_compose_multiply()
    {
        var square =
            Compose(Multiply.Curried()(3), Length);
        Assert.Equal(9, square("foo"));
    }

    [Fact]
    internal void check_compute_2()
    {
        var square =
            Compose(
                Compute2(5)(6),
                Length);
        Assert.Equal(23, square("foo"));
    }

    [Fact]
    internal void check_compute()
    {
        var curried = Curried<int, int, int, int>(5, Compute);
        var curried6 = Curried(6, curried);
        var square =
            Compose(
                curried6,
                Length);
        Assert.Equal(23, square("foo"));
    }
}

static class Extensions
{
    internal static Func<A, C> Compose<A, B, C>(Func<B, C> f, Func<A, B> g) =>
        a => f(g(a));

    internal static Func<B, C> Curried<A, B, C>(A a, Func<A, B, C> f) =>
        b => f(a, b);

    internal static Func<A, Func<B, C>> Curried<A, B, C>(this Func<A, B, C> f) => a => b => f(a, b);
    

    internal static Func<B, C, D> Curried<A, B, C, D>(A a, Func<A, B, C, D> f) =>
        (b, c) => f(a, b, c);

    internal static Func<string, int> Length = word =>
        word.Length;

    internal static Func<string, string> Duplicate=word =>
        word + word;

    internal static Func<int, int> Square=length =>
        (int)Math.Pow(length, 2);

    internal static Func<int, int, int> Multiply = (x, y) =>
        x * y;

    internal static int Compute(int x, int y, int z) =>
        x + y * z;

    internal static Func<int, Func<int, int>> Compute2(int x) => y => z =>
        x + y * z;
}
