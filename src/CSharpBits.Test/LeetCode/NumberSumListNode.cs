using System.Collections.Generic;
using Xunit;
using static CSharpBits.Test.LeetCode.Helpers;

namespace CSharpBits.Test.LeetCode;

public class Quiz2AddTwoNumbersTestListNode
{
    [Fact]
    void stringify()
    {
        List<int> exp = [7, 0, 9];
        var s = exp.ToListNode().Stringify();

        Assert.Equal("709", s);
    }

    [Fact]
    internal void Sum_of_3_items_lists_with_ListNode()
    {
        List<int> one = [2, 4, 5];
        List<int> two = [5, 6, 4];
        List<int> exp = [7, 0, 0, 1];

        var res = Sum(one.ToListNode(), two.ToListNode());

        Assert.Equal("7001", res.Stringify());
    }

    [Fact]
    internal void Sum_of_3_4_items_lists_with_ListNode()
    {
        List<int> one = [2, 4, 5, 3];
        List<int> two = [5, 6, 4];
        List<int> exp = [7, 0, 0, 4];

        var res = Sum(one.ToListNode(), two.ToListNode());

        Assert.Equal("7004", res.Stringify());
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(9, 9, 0)]
    [InlineData(10, 0, 10)]
    [InlineData(11, 1, 10)]
    [InlineData(18, 8, 10)]
    void floor10(int n, int floored, int remainder)
    {
        var (f, r) = n.Floor10();

        Assert.Equal(floored, f);
        Assert.Equal(r, remainder);
    }
}

file static class Helpers
{
    internal static string Stringify(this ListNode listNode)
    {
        if (listNode.IsEmpty()) return "";
        else return $"{listNode.Val}{listNode.Next.Stringify()}";
    }

    internal static ListNode ToListNode(this List<int> l) =>
        l.Count == 0
            ? null
            : new ListNode(l[0], l[1..].ToListNode());

    internal static (int, int) Floor10(this int n)
    {
        var m = n % 10;
        var remainder = n - m;
        return (m, remainder);
    }

    private static bool IsEmpty(this ListNode l) => l == null;

    private static int NVal(this ListNode l) => l.IsEmpty() ? 0 : l.Val;
    private static ListNode NNext(this ListNode l) => l.IsEmpty() ? null : l.Next;

    private static ListNode SumAcc(ListNode l1, ListNode l2, int remainder)
    {
        if (l1.IsEmpty() && l2.IsEmpty())
            return remainder > 0
                ? new ListNode(remainder)
                : null;

        var sum = l1.NVal() + l2.NVal() + remainder;
        var (res, r) = sum.Floor10();

        return new ListNode(res, SumAcc(l1.NNext(), l2.NNext(), r / 10));
    }

    internal static ListNode Sum(ListNode l1, ListNode l2)
    {
        return SumAcc(l1, l2, 0);
    }
}

// Provided by LeetCode
file class ListNode
{
    internal readonly int Val;
    internal readonly ListNode Next;

    internal ListNode(int val = 0, ListNode next = null)
    {
        Val = val;
        Next = next;
    }
}
