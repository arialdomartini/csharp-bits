using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FsCheck;
using Xunit;
using static System.Array;

public class QuickSortTest
{
    
    
    // fact 1 = 1
    // fact n = n * fact(n-1)
        

    //  sorted [] = []
    //  sorted x:xs
    //               smaller  :: x :: bigger 
    //               where
    //                 smaller = sorted [ e | e <<- xs , e<x ]
    //                 bigger  = sorted [e | e <<- xs , e>=x]
    
    
    
    [Fact]
    void quick_sort()
    {
        Assert.Equal(Empty<int>(), Empty<int>().sorted());
        Assert.Equal(new[] { 1, 2, 3 }, new[] { 3, 2, 1 }.sorted());
        Assert.Equal(new[] { 1, 2, 3, 10,33 }, new[] { 10, 33, 3, 2, 1 }.sorted());
    }
}

internal static class QuickSort
{
    internal static IEnumerable<int> sorted(this IEnumerable<int> xs)
    {
        if (xs.IsEmpty())
            return Empty<int>();
        
        var (pivot, rest) = xs.Pivot();
        var (smaller, bigger) = rest.Partition(e => e < pivot);
        
        return 
            smaller.sorted()
            .Concat(pivot)
            .Concat(bigger.sorted());
    }

    private static IEnumerable<int> Concat(this IEnumerable<int> list, int element) => 
        list.Concat(new [] { element });

    private static (IEnumerable<int>, IEnumerable<int>)
        Partition(this IEnumerable<int> xs, Func<int, bool> predicate)
    {
        var matches = xs.Where(predicate);
        var doesNotMatches = xs.Where(x => !predicate(x));
        return (matches, doesNotMatches);
    }
    
    
    private static IEnumerable<int> SmallerThan(this IEnumerable<int> rest, int pivot) => 
        SubSetOn(rest, x => x < pivot);

    private static IEnumerable<int> SubSetOn(IEnumerable<int> list, Func<int, bool> predicate) => 
        list.Where(predicate);

    private static IEnumerable<int> BiggerThan(this IEnumerable<int> rest, int pivot) => 
        SubSetOn(rest, x => x >= pivot);

    private static (int, IEnumerable<int>) Pivot(this IEnumerable<int> xs) => 
        (xs.First(), xs.Skip(1));

    private static bool IsEmpty(this IEnumerable<int> xs) => 
        !xs.Any();
} 
