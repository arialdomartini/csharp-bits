using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ReaderMonad.Unfunctional;

public class QuasiReaderMonadTest
{
    [Fact]
    void composing_functions()
    {
        Func<int, int> square = n => n * n;
        Func<int, string> evenOrOdd = n => n % 2 == 0 ? "even":"odd";

        Assert.Equal("even", evenOrOdd(square(10)));
        Assert.Equal("odd", evenOrOdd(square(5)));
    }

    [Fact]
    void adding_log_to_each_method()
    {
        var logs = new List<string>();
        void Log(string message) =>
            logs.Add(message);

        Func<int, int> square = n =>
        {
            Log($"invoked with parameter {n}");
            return n * n;
        };

        Func<int, string> evenOrOdd = n =>
        {
            Log($"invoked with parameter {n}");
            return n % 2 == 0 ? "even" : "odd";
        };

        var result = evenOrOdd(square(10));

        Assert.Equal("even", result);

        logs.Should().BeEquivalentTo(
            "invoked with parameter 10",
            "invoked with parameter 100");
    }

    [Fact]
    void composed_function_as_a_named_function()
    {
        var logs = new List<string>();
        void Log(string message) =>
            logs.Add(message);

        Func<int, int> square = n =>
        {
            Log($"invoked with parameter {n}");
            return n * n;
        };

        Func<int, string> evenOrOdd = n =>
        {
            Log($"invoked with parameter {n}");
            return n % 2 == 0 ? "even" : "odd";
        };

        Func<int, string> composed = n => evenOrOdd(square(n));

        var result = composed(10);

        Assert.Equal("even", result);

        logs.Should().BeEquivalentTo(
            "invoked with parameter 10",
            "invoked with parameter 100");
    }

    [Fact]
    void composition_of_functions_as_a_function()
    {
        var logs = new List<string>();
        void Log(string message) =>
            logs.Add(message);

        Func<int, int> square = n =>
        {
            Log($"invoked with parameter {n}");
            return n * n;
        };

        Func<int, string> evenOrOdd = n =>
        {
            Log($"invoked with parameter {n}");
            return n % 2 == 0 ? "even" : "odd";
        };

        Func<int, string> compose(Func<int, int> first, Func<int, string> second) =>
            n => second(first(n));

        Func<Func<int, int>, Func<int, string>, Func<int, string>> compose2 =
            (first, second) =>
                n =>
                    second(first(n));

        Func<int, string> composed = compose(square, evenOrOdd);

        var result = composed(10);

        Assert.Equal("even", result);

        logs.Should().BeEquivalentTo(
            "invoked with parameter 10",
            "invoked with parameter 100");
    }
}