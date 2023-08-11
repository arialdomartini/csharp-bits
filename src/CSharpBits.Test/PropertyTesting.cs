using System.Linq;
using Xunit;

namespace CSharpBits.Test;

using FsCheck;
using FsCheck.Xunit;
using static FsCheck.Prop;

public class PropertyTesting
{
    [Property]
    Property square_of_numbers_are_non_negative()
    {
        var numbers = Arb.From<int>();

        int square(int n) => n * n;

        bool squareIsNotNegative(int n) => square(n) >= 0;

        return ForAll(numbers, squareIsNotNegative);
    }

    [Fact]
    void square_of_numbers_are_non_negative_as_a_fact()
    {
        Arbitrary<int> numbers = Arb.From<int>();

        int square(int n) => n * n;

        bool squareIsNotNegative(int n) => square(n) >= 0;

        Property property = ForAll(numbers, squareIsNotNegative);
        
        Check.QuickThrowOnFailure(property);
    }

    [Fact]
    void what_is_an_Arb()
    {
        Arbitrary<int> arbitraryNumber = Arb.From<int>();

        var ns = Gen.Sample<int>(50, 100, arbitraryNumber.Generator).ToList();
        
        Assert.Equal(100, ns.Count);
        Assert.True(ns.TrueForAll(n => n <= 50));
    }
    
    [Property]
    Property square_of_numbers_are_non_negative_one_liner() => 
        ForAll(Arb.From<int>(), n => n * n >= 0);
}
