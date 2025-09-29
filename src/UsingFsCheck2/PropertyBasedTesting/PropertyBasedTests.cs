using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace CSharpBits.Test.PropertyBasedTesting;

public class PropertyBasedTests
{
    [Property]
    Property square_of_numbers_are_non_negative()
    {
        var numbers = Arb.From<int>();

        int square(int n) => n * n;

        bool squareIsNotNegative(int n) => square(n) >= 0;

        return Prop.ForAll(numbers, squareIsNotNegative);
    }

    [Fact]
    void square_of_numbers_are_non_negative_as_a_fact()
    {
        Arbitrary<int> numbers = Arb.From<int>();

        int square(int n) => n * n;

        bool squareIsNotNegative(int n) => square(n) >= 0;

        Property property = Prop.ForAll(numbers, squareIsNotNegative);
        
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

    void Foo(string a, int b, double c)
    {
        
    }
    
    [Property]
    Property square_of_numbers_are_non_negative_one_liner() => 
        Prop.ForAll(Arb.From<int>(), n => n * n >= 0);
}
