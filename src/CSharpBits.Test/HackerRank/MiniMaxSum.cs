using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.HackerRank;

public class MiniMaxSum
{
    [Fact]
    void example()
    {
        List<int> xs = [1, 3, 5, 7, 9];

        var (min, max) = ListExtension.Calculate(xs);

        Assert.Equal(16, min);
        Assert.Equal(24, max);
    }

    [Fact]
    void example2()
    {
        List<int> xs = [    256741038, 623958417, 467905213, 714532089, 938071625];

        var (min, max) = ListExtension.Calculate(xs);

        Assert.Equal(2063136757, min);
        Assert.Equal(2744467344, max);
    }
}

internal static class ListExtension
{
    private static List<int> Without(this List<int> xs, int index) =>
        xs
            .Zip(Enumerable.Range(0, xs.Count))
            .Where((n, i) => i != index)
            .Select(el => el.Item1)
            .ToList();

    internal static (long, long) Calculate(List<int> arr) => Solution2(arr);

    private static (long, long) Solution2(List<int> arr)
    {
        long min = arr.Min();
        long max = arr.Max();
        long sum = arr.Aggregate(0, (long s, int i) => s + (long)i);

        return (sum - max, sum - min);
    }
    private static (int, int) Solution1(List<int> arr)
    {
        var fourElementLists =
            from index in Enumerable.Range(0, arr.Count)
            select arr.Without(index);

        var sums = fourElementLists.Select(l => l.Sum()).ToList();

        return (sums.Min(), sums.Max());
    }
}

class Result
{
    internal static void miniMaxSum(List<int> arr)
    {
        var r = ListExtension.Calculate(arr);
        Console.WriteLine($"{r.Item1} {r.Item2}");
    }
}
