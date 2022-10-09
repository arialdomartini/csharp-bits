using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
namespace CSharpBits.Test;

public class RpnTest
{
    [Fact]
    void calculates_single_sum()
    {
        Assert.Equal(5, "2 3 +".Eval());
    }
    
    [Fact]
    void calculates_series_of_sums()
    {
        Assert.Equal(13, "2 3 + 4 4 + +".Eval());
    }
}

internal static class RpnCalculator
{
    private static readonly Dictionary<string, Func<double, double, double>> Operations = new()
    {
        ["+"] = (a, b) => a + b,
        ["-"] = (a, b) => a - b,
        ["*"] = (a, b) => a * b,
        ["/"] = (a, b) => a / b
    };

    internal static double Eval(this string rawInput)
    {
        Func<IEnumerable<double>, string, IEnumerable<double>> aggregator = (acc, i) => 
            Operations.ContainsKey(i) ?
                EvalOperation(acc, Operations[i]) : 
                acc.Append(double.Parse(i));

        return rawInput
            .Tokenize()
            .Aggregate(new List<double>(), aggregator)
            .Max();
    }

    private static IEnumerable<double> EvalOperation(IEnumerable<double> acc, Func<double, double, double> operation)
    {
        var v1 = acc.First();
        var v2 = acc.Skip(1).First();
        return acc.Skip(2).Append(operation(v1, v2));
    }

    internal static IEnumerable<string> Tokenize(this string rawInput) =>
        rawInput.Split(" ");
}
