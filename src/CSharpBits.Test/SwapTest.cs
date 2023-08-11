using System;
using CSharpBits.Test.ReaderMonad.ToReaderMonad.Step1;
using Xunit;

public class SwapTest
{

    [Fact]
    void swap_test()
    {
        Func<string, int, bool> f = (s, i) => s.Length == i;
        
        // Func<int, string, bool> g = (i, s) => s.Length == i;

        Func<B, A, C> swap<A, B, C>(Func<A, B, C> f) => (b, a) => f(a, b);

        var g = swap(f);

        Assert.Equal(f("luca", 4), g(4, "luca"));
        Assert.Equal(f("giovanni", 4), g(4, "giovanni"));
    }
    
    [Fact]
    void twice_test()
    {
        Func<string, string> addAsterisk = s => "*" + s + "*";


        var addDoubleAsterisk = addAsterisk.twice();
        
        Assert.Equal("**lorenzo**", addDoubleAsterisk("lorenzo"));
        
    }
    
    [Fact]
    void unless_test()
    {
        Func<decimal, string> f = d => d.ToString();
        Func<decimal, string> g = f.unless(i => i == "0", "zero");
        
        Assert.Equal(f(0), "0");
        Assert.Equal(f(1), "1");
        Assert.Equal(f(1999), "1999");
        
        Assert.Equal(g(0), "zero");
        Assert.Equal(g(1), "1");
        Assert.Equal(g(1999), "1999");



    }
    // Func<decimal, string> pay
    // Func<decimal, string> safePay = pay.Unless(containsPan, then: mask);
    
}
internal static class TwiceExtensions
{
    internal static Func<string, string> twice(this Func<string, string> f) => 
        s => f(f(s));
    
    internal static Func<decimal, string> unless(this Func<decimal, string> f, Func<string, bool> predicate, string exceptionalCase) =>
        d =>
        {
            var result = f(d);
            if (predicate(result))
                return exceptionalCase;
            return result;
        };
}
