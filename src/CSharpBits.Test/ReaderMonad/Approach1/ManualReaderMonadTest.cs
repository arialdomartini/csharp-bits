using System;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ReaderMonad.Approach1;

using Env = Int32;

file static class ReaderExtensions
{
    internal static Func<A, Func<B, C>> Curried<A, B, C>(this Func<A, B, C> f) =>
        a => b => f(a, b);

    internal static A Run<E, A>(this Func<E, A> reader, E env) =>
        reader(env);

    internal static Func<E, B> Map<E, A, B>(this Func<E, A> reader, Func<A, B> f) =>
        env => f(reader.Run(env));

    internal static Func<E, B> Bind<E, A, B>(this Func<E, A> reader, Func<A, Func<E, B>> f) =>
        env => f(reader(env)).Run(env);
}

internal static class LinqExtensions
{
    internal static Func<E, C> SelectMany<E, A, B, C>(
        this Func<E, A> reader,
        Func<A, Func<E, B>> bind,
        Func<A, B, C> project) =>
        env =>
        {
            A a = reader.Run(env);
            Func<E, B> func = bind(a);
            B b = func(env);
            var c = project(a, b);
            return c;
        };
}

public class ManualReaderMonadTest
{
    string Greet(string name, Env env) =>
        $"I'm greeting {name} while Env={env}";

    [Fact]
    void run_a_function()
    {
        Func<string, Env, string> func = Greet;

        var greet = func.Curried();

        var applied = greet("Mario");

        var result = applied.Run(42);

        result.Should().Be("I'm greeting Mario while Env=42");
    }

    [Fact]
    void apply_a_function_to_a_function_like_functor_map()
    {
        string ToLower(string s) =>
            s.ToLower();

        Func<string, int, string> func = Greet;
        var greet = func.Curried();

        var result =
            greet("Mario")
                .Map(ToLower)
                .Run(42);

        result.Should().Be("i'm greeting mario while env=42");
    }

    [Fact]
    void bind_a_function()
    {
        string Second(string s, Env env) =>
            env > 42 ? s.ToLower() : s.ToUpper();

        Func<string, Env, string> second = Second;

        var curriedSecond = second.curried();

        Func<string, int, string> func = Greet;
        var greet = func.Curried();

        var result =
            greet("Mario")
                .Bind(curriedSecond)
                .Run(42);

        result.Should().Be("I'M GREETING MARIO WHILE ENV=42");
    }

    [Fact]
    void using_linq()
    {
        string Second(string s, Env env) =>
            env > 42 ?  s.ToUpper() : s.ToLower();

        Func<string, Env, string> second = Second;

        var curriedSecond = second.curried();

        Func<string, int, string> func = Greet;
        var greet = func.Curried();

        var result =
            from x in greet("Mario")
            from y in curriedSecond(x)
            select $"{y}!!";

        result.Run(42)
            .Should().Be("i'm greeting mario while env=42!!");
    }
}
