using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;

namespace CSharpBits.Test.PropertyBasedTesting;

internal static class PrimeNumbersExtensions
{
    internal static bool IsPrime(this int n) => 
        Enumerable.Range(1, n)
            .Where(i => !(i == 1 || i == n))
            .All(i => n.CannotBeDividedBy(i));

    private static bool CannotBeDividedBy(this int n, int i) => 
        n % i != 0;

    internal static bool AreAllPrime(this IEnumerable<int> xs) => 
        xs.All(IsPrime);

    internal static int Multiplied(this IEnumerable<int> xs) => 
        xs.Aggregate(1, (product, i) => product * i);
}

public class PrimeFactorTest
{
    private static readonly Func<int, IEnumerable<int>> factorize = n =>
    {
        if (n.IsPrime() ) return new[] { n };

        var factor = findFactor(n);
        return new[] { factor }.Concat(factorize(n / factor)); 
    };

    private static int findFactor(int n)
    {
        for (var i = 2; i < n; i++)
        {
            if (n % i == 0) return i;
        }

        return n; }

    private readonly Arbitrary<int> PositiveNumbers = Arb.Generate<int>().Select(Math.Abs).Where(n => n > 0).ToArbitrary();

    [Property]
    Property factorization_in_prime_numbers()
    {
        bool isFactorizedInPrimeNumbers(int n)
        {
            var factors = factorize(n);
            
            return factors.AreAllPrime() && factors.Multiplied() == n;
        }

        return Prop.ForAll(PositiveNumbers, isFactorizedInPrimeNumbers);
    }
}
