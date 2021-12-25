using System;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ManualReaderMonad
{
    using Env = Int32;

    public class ManualReaderMonadTest
    {
        string Greet(string name, Env env) =>
            $"I'm greeting {name} while Env={env}";

        Func<Env, string> CurriedGreet(string name) => (Env env) =>
            $"I'm greeting {name} while Env={env}";

        Func<A, Func<B,C>> Curry<A, B, C>(Func<A, B, C> f) =>
            a => b => f(a, b);

        [Fact]
        void run_a_function()
        {
            var greet = Curry<string, Env, string>(Greet);

            var applied = greet("Mario");

            var result = applied(42);

            result.Should().Be("I'm greeting Mario while Env=42");
        }
    }
}