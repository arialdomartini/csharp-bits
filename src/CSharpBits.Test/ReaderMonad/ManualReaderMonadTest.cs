﻿using System;
using System.Net.NetworkInformation;
using CSharpBits.Test.ReaderMonad;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ManualReaderMonad
{
    using Env = Int32;

    internal static class ReaderExtensions
    {
        internal static Func<A, Func<B, C>> Curried<A, B, C>(this Func<A, B, C> f) =>
            a => b => f(a, b);

        internal static A Run<E, A>(this Func<E, A> reader, E env) =>
            reader(env);

        internal static Func<E, B> Map<E, A, B>(this Func<E, A> reader, Func<A, B> f) =>
            env => f(reader.Run(env));
    }

    public class ManualReaderMonadTest
    {
        string Greet(string name, Env env) =>
            $"I'm greeting {name} while Env={env}";

        [Fact]
        void run_a_function()
        {
            Func<string, int, string> func = Greet;

            var greet = func.Curried();

            var applied = greet("Mario");

            var result = applied.Run(42);

            result.Should().Be("I'm greeting Mario while Env=42");
        }

        [Fact]
        void apply_a_function_to_a_function_like_functor_map()
        {
            Func<string, string> toLower = s => s.ToLower();

            Func<string, int, string> func = Greet;

            var greet = func.Curried();

            var result = greet("Mario").Map(toLower).Run(42);

            result.Should().Be("i'm greeting mario while env=42");
        }
    }
}