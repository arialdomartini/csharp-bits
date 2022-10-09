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

    internal static double Eval(this string rawInput) =>
        rawInput
            .Tokenize()
            .Aggregate(
                Array.Empty<double>(), (
                    Func<double[], string, double[]>)((acc, i) =>
                    Operations.ContainsKey(i) ? 
                        EvalOperation(acc, Operations[i]) : 
                        acc.Append(double.Parse(i)).ToArray()))
            .Max();

    private static double[] EvalOperation(double[] acc, Func<double, double, double> operation) =>
        acc.Skip(2).Append(operation(acc[0], acc[1])).ToArray();

    private static IEnumerable<string> Tokenize(this string rawInput) =>
        rawInput.Split(" ");
}
