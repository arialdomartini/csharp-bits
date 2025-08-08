using System.Collections.Generic;
using System.Linq;
using Xunit;
using static CSharpBits.Test.LeetCode.Quiz2AddTwoNumbers;

namespace CSharpBits.Test.LeetCode;

public class Quiz2AddTwoNumbersTest
{
    [Fact]
    internal void Sum_of_3_items_lists()
    {
        List<int> one = [2, 4, 3];
        List<int> two = [5, 6, 4];
        List<int> exp = [7, 0, 8];

        var res = Sum(one, two);

        Assert.Equivalent(exp, res);
    }

    [Fact]
    internal void Sum_of_3_items_lists_with_last_remainder()
    {
        List<int> one = [2, 4, 5];
        List<int> two = [5, 6, 4];
        List<int> exp = [7, 0, 0, 1];

        var res = Sum(one, two);

        Assert.Equivalent(exp, res);
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(9, 9, 0)]
    [InlineData(10, 0, 10)]
    [InlineData(11, 1, 10)]
    [InlineData(18, 8, 10)]
    void Test_floor10(int n, int floored, int remainder)
    {
        var (f, r) = n.Floor10();

        Assert.Equal(floored, f);
        Assert.Equal(r, remainder);
    }
}

file static class Quiz2AddTwoNumbers
{
    internal static (int, int) Floor10(this int n)
    {
        var m = n % 10;
        var remainder = n - m;
        return (m, remainder);
    }

    private static (int, List<int>) Calc((int Remainder, List<int> Result) acc, (int Left, int Right) el)
    {
        var sum = el.Left + el.Right + acc.Remainder;
        var (s, r) = sum.Floor10();

        return (r / 10, acc.Result.Append([s]).ToList());
    }

    private static (int, List<int>) Sum(this IEnumerable<(int, int)> zs) =>
        zs.Aggregate((0, new List<int>()), Calc);

    internal static List<int> Sum(List<int> l1, List<int> l2)
    {
        var (r, res) = l1.Zip(l2).Sum();
        return r > 0
            ? res.Append([r]).ToList()
            : res;
    }
}
