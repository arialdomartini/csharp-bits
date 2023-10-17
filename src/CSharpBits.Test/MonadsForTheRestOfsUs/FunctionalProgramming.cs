using System;
using Xunit;

namespace Functional;

public class FunctionalProgramming
{
    [Fact]
    public void check_functional_composition()
    {
        var foo = "foo";

        var square = Extensions.Compose<string, int, int>(Extensions.GetSquare, Extensions.GetLength);

        Assert.Equal(9, square(foo));
    }

    [Fact]
    public void check_duplicate_string()
    {
        var foo = "foo";

        var square = Extensions.Compose<string, string, int>(Extensions.GetLength, Extensions.DuplicateString);
        Assert.Equal(6, square(foo));
    }

    [Fact]
    public void check_compose_multiply()
    {
        var square =
            Extensions.Compose<string, int, int>(Extensions.Carried<int, int, int>(3, Extensions.Multiply), Extensions.GetLength);
        Assert.Equal(9, square("foo"));
    }

    [Fact]
    public void check_compute_2()
    {
        var square =
            Extensions.Compose<string, int, int>(
                Extensions.Compute2(5)(6),
                Extensions.GetLength);
        Assert.Equal(23, square("foo"));
    }

    [Fact]
    public void check_compute()
    {
        Func<int, int, int> carried = Extensions.Carried<int, int, int, int>(5, Extensions.Compute);
        var carried6 = Extensions.Carried(6, carried);
        var square =
            Extensions.Compose<string, int, int>(
                carried6,
                Extensions.GetLength);
        Assert.Equal(23, square("foo"));
    }
}

static class Extensions
{
    public static Func<TIn, TOut> Compose<TIn, T, TOut>(Func<T, TOut> f, Func<TIn, T> g) =>
        p => f(g(p));

    public static Func<TP2, TOut> Carried<TP1, TP2, TOut>(TP1 p1, Func<TP1, TP2, TOut> f) =>
        p2 => f(p1, p2);

    public static Func<TP2, TP3, TOut> Carried<TP1, TP2, TP3, TOut>(TP1 p1, Func<TP1, TP2, TP3, TOut> f) =>
        (p2, p3) => f(p1, p2, p3);

    public static int GetLength(string word) =>
        word.Length;

    public static string DuplicateString(string word) =>
        word + word;

    public static int GetSquare(int length) =>
        (int)Math.Pow(length, 2);

    public static int Multiply(int x, int y) =>
        x * y;

    public static int Compute(int x, int y, int z) =>
        x + y * z;

    public static Func<int, Func<int, int>> Compute2(int x) => y => z =>
        x + y * z;
}
