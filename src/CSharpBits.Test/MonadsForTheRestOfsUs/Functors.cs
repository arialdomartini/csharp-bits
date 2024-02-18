using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Xunit;

namespace CSharpBits.Test.MonadsForTheRestOfsUs.MyOptionDeferred;

internal static class FunctorExtensions
{
}

public class Functors
{
    [Fact]
    void functor_as_a_mapper()
    {
        string s = "foo";
        Func<string, int> length = s => s.Length;

        IEnumerable<string> values = new[] { "foo", "barbaz", "" };

        IEnumerable<int> lengths = values.Select(length);

        Assert.Equal(new[] { 3, 6, 0 }, lengths);
    }

    [Fact]
    void hashmaps()
    {
        var hm1 = new HashMap<string, int>
        {
            { "foo", 42 },
            { "bar", 100 }
        };
        var hm2 = new HashMap<string, int>
        {
            { "foo", 42337 },
            { "barz", 10033 }
        };

        Assert.True(hm1.Equals(hm2));
    }
    
    [Fact]
    void records_containing_hashmaps()
    {
        var hm1 = new HashMap<string, int>
        {
            { "foo", 42 },
            { "bar", 100 }
        };
        var hm2 = new HashMap<string, int>
        {
            { "foo", 42 },
            { "bar", 100 }
        };
        var r1 = new MyRecordHM("foo", hm1);
        var r2 = new MyRecordHM("foo", hm2);

        Assert.True(hm1.Equals(hm2));
        Assert.True(r1.Equals(r2));
    }
}

public record MyRecord(string Name, Dictionary<string, int> Keys);

public record MyRecordHM(string Name, HashMap<string, int> Keys);
