using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace UsingFsCheck2;

public class PrachiKata1Property
{

// Write a function that takes an integer and returns the collection of its prime factors;
//
// Factors of a number are integers that,
// when multiplied together, result in the original number.
//
// A number is prime if it has no divisors other than 1 and itself

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


    [Property]
    void the_returned_factors_multiplied_are_the_original_number(PositiveInt originalNumber)
    {
        var multiplied = PrimeFactors(originalNumber.Get).Multiplied();

        Assert.Equal(
            multiplied,
            originalNumber.Get);
    }

    [Property(MaxTest = 1_000_000)]
    void all_the_returned_factors_are_prime(PositiveInt originalNumber)
    {
       Assert.True(PrimeFactors(originalNumber.Get).AllPrimePrime());
    }
}


static class TestHelpers
{
    internal static int Multiplied(this List<int> primeFactors) => primeFactors.Aggregate(1, (i, i1) => i * i1);

    internal static bool AllPrimePrime(this List<int> primeFactors)
    {
        return primeFactors.All(primeFactor => primeFactor.IsPrime());
    }

    internal static bool IsPrime(this int n)
    {
        return
            Enumerable.Range(1, n)
                .Skip(1)
                .SkipLast()
                .All(i => n.NotDivisibleBy(i));
    }

    private static bool NotDivisibleBy(this int n, int i)
    {
        if (n % i != 0) return true;
        return false;
    }
}
