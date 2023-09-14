//using Hedgehog;

//using Hedgehog;

using System.Linq;
using Hedgehog;
using Hedgehog.Linq;
using Hedgehog.Xunit;
using Xunit;
using Gen = Hedgehog.Linq.Gen;

//using Gen = Hedgehog.Linq.Gen;

namespace CSharpBits.Test.PropertyBasedTesting.WithHedgehog;

public class PrimeFactorTests
{
    [Property]
    void should_fail()
    {
        var stringLength = 10;
        
        var stringGen = Hedgehog.Gen.alpha.String(Hedgehog.Linq.Range.FromValue(stringLength));

        // This creates a property..
        var property =
            from str in Hedgehog.Linq.Property.ForAll(stringGen)
            select str.Length == stringLength + 1;

        // ..that can be checked, rechecked or rendered.
        property.Check();
    }

    [Fact]
    void sample_of_numbers()
    {
        var numbersGenerator = Gen.Int32(Hedgehog.Range.constant(1,50));

        var numbers = Hedgehog.Gen.sample(0, 100, numbersGenerator);

        Assert.Equal(100, numbers.Count());
        Assert.True(numbers.All(n => n >= 1));
        Assert.True(numbers.All(n => n <= 50));
    }
    
    
    [Fact]
    void strings_from_chars()
    {
        Gen<char> chars = Gen.Alpha;
        Range<int> upTo50 = Hedgehog.Range.constant(1,50);

        var stringsGenerator = Hedgehog.Gen.seq(upTo50, chars);

        var strings = Hedgehog.Gen.sample(0, 100, stringsGenerator).Select(c => string.Join("", c));

        Assert.Equal(100, strings.Count());
        Assert.True(strings.All(s => s.Length >= 1));
    }
    
    

    [Property]
    bool square_is_always_positive(int n) => 
        (n * n) >= 0;

    [Property]
    bool sum_is_commutative(int a, int b) => 
        a + b == b + a;
}
