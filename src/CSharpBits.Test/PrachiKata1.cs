using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using Xunit;
using System.Linq;

namespace CSharpBits.Test;

// Write a function that takes an integer and returns the collection of its prime factors;
//
// Factors of a number are integers that,
// when multiplied together, result in the original number.
//
// A number is prime if it has no divisors other than 1 and itself

public class PrachiKata1
{
    List<int> PrimeFactors(int n)
    {
        List<int> factors = [];

        var remainder = n;

        var divisor = 2;

        while(remainder > 1)
        {
            while (remainder % divisor == 0)
            {
                factors.Add(divisor);
                remainder /= divisor;
            }

            divisor++;
        }

        return factors;
    }


    [Fact]
    void all_cases()
    {
        Assert.Equal([], PrimeFactors(1));
        Assert.Equal([2], PrimeFactors(2));
        Assert.Equal([3], PrimeFactors(3));
        Assert.Equal([2,2], PrimeFactors(4));
        Assert.Equal([5], PrimeFactors(5));
        Assert.Equal([2,3], PrimeFactors(6));
        Assert.Equal([7], PrimeFactors(7));
        Assert.Equal([2,2,2], PrimeFactors(8));
        Assert.Equal([3,3], PrimeFactors(9));
        Assert.Equal([2,5], PrimeFactors(10));
        Assert.Equal([11], PrimeFactors(11));
        Assert.Equal([2,2,3], PrimeFactors(12));
        Assert.Equal([5,5], PrimeFactors(25));
        Assert.Equal([2,2,5,5], PrimeFactors(100));
    }


    [Fact]
    void scanl_equivalent()
    {
        var l = new List<int> { 1, 2, 3 };
        int sum = 0;
        List<(int x, int)> result = l.Select(x => (x, sum += x)).ToList();

        Assert.Equal([(1, 1), (2, 3), (3, 6)], result);
    }




    [Fact]
    void usage_of_scanl()
    {
        var l = new List<int> { 1, 2, 3 };
        var cumulatedSums = l.Scanl(0, (s, e) => s + e);
        Assert.Equal([1, 3, 6], cumulatedSums);
        Assert.Equal([(1, 1), (2, 3), (3, 6)], l.Zip(cumulatedSums));
    }

}

public static class ScanlExtension
{
    public static IEnumerable<TAccumulate> ScanlMutation<TSource, TAccumulate>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
    {
        foreach (var item in source)
        {
            seed = func(seed, item);
            yield return seed;
        }
    }

    public static IEnumerable<TAcc> Scanl<T, TAcc>(
        this IEnumerable<T> xs,
        TAcc acc,
        Func<TAcc, T, TAcc> f) =>
        xs.Aggregate(
            new List<TAcc> { acc },
            (list, item) => new List<TAcc> { f(list[0], item) }.Concat(list).ToList()
        ).AsEnumerable().Reverse().Skip(1);
}
