using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;

namespace CSharpBits.Test.PropertyBasedTesting;

public class ListReversalPropertyTests
{
    static bool AreListsEqual<T>(IEnumerable<T> l1, IEnumerable<T> l2)
    {
        var list1 = l1.ToList();
        var list2 = l2.ToList();

        if (list1.Count != list2.Count)
            return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                return false;
        }

        return true;
    }

    [Property]
    bool invariant_of_reversal(List<string> xs) => 
        AreListsEqual(xs, xs.AsEnumerable().Reverse().Reverse());

    [Property]
    bool specification_of_reversal(List<string> xs)
    {
        var reversed = Reverse(xs).ToList();

        // if (xs.Count != reversed.Count) return false;

        var eachItemHasBeenReversed =
            Enumerable.Range(0, xs.Count)
                .All(i => xs[i] == reversed[xs.Count - i - 1]);

        return eachItemHasBeenReversed;
    }

    private static IEnumerable<string> Reverse(IEnumerable<string> xs) => 
        xs.Reverse();

    // private static IEnumerable<string> Reverse(IEnumerable<string> xs) => 
        // xs;
}
