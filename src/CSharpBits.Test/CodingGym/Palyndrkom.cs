using System;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.CodingGym;

public class Palyndrom
{
    (string, string) cut(string s) =>
        s.Length % 2 == 0
            ? (
                s.Substring(0, s.Length / 2),
                s.Substring(s.Length / 2, s.Length / 2))
            : (s.Substring(0, s.Length / 2),
                (s.Substring(s.Length / 2 + 1, s.Length / 2)));

    [Theory]
    [InlineData("abba", "ab", "ba")]
    [InlineData("abbac", "ab", "ac")]
    void cut_string(string s, string el, string er)
    {
        var (l, r) = cut(s);

        Assert.Equal(er, r);
        Assert.Equal(el, l);
    }

    [Theory]
    [InlineData('a', 'a', 0)]
    [InlineData('a', 'b', 1)]
    [InlineData('b', 'a', 1)]
    [InlineData('d', 'b', 2)]
    [InlineData('b', 'd', 2)]
    void cut_reverse_string(char cl, char cr, int expectedDistance)
    {
        var d = distance(cl, cr);

        Assert.Equal(expectedDistance, d);
    }

    [Fact]
    void rev()
    {
        var s = "foobar".Reverse();      
        
        Assert.Equal("raboof", s);
    }
    
    [Theory]
    [InlineData("abba", 0)]
    [InlineData("abxba", 0)]
    [InlineData("abxbb", 1)]
    [InlineData("abxcb", 2)]
    [InlineData("bbcb", 1)]
    [InlineData("b", 0)]
    void calculate_min_number_of_operations(string s, int expected)
    {
        var (l, r) = cut(s);
        var d = Enumerable.Zip(l, r.Reverse(), distance).Sum();
        
        Assert.Equal(expected, d);
        
    }

    [Theory]
    [InlineData("abba", 0)]
    [InlineData("abxba", 0)]
    [InlineData("abxbb", 1)]
    [InlineData("abxcb", 2)]
    [InlineData("bbcb", 1)]
    [InlineData("b", 0)]
    void oneline(string s, int expected)
    {
        var (l, r) = cut(s);
        var r2 = r.Reverse();
        var d = l.Zip(r2, distance).Sum();
        Assert.Equal(expected, d);
        
    }

    private int distance(char cl, char cr) =>
        Math.Abs(cl - cr);
}
