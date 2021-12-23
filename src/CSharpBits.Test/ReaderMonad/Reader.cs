using System;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ReaderMonad
{
    using Env = Int32;

    class Reader<TEnv, TResult>
    {
        private readonly Func<TEnv, TResult> _f;

        private Reader(Func<TEnv, TResult> f)
        {
            _f = f;
        }

        internal static Reader<TEnv, TResult> From(Func<TEnv, TResult> f) =>
            new Reader<TEnv, TResult>(f);

        internal TResult Run(TEnv i) =>
            _f(i);

        internal Reader<TEnv, TResultG> Bind<TResultG>(Func<TResult, Reader<TEnv, TResultG>> g)
        {
            Func<TEnv, TResultG> rf =
                env =>
                    g(_f(env))
                        .Run(env);
            return rf.ToReader();
        }

        internal Reader<TEnv, TResultG> Map<TResultG>(Func<TResult, TResultG> g)
        {
            Func<TEnv, TResultG> rg = env =>
                g(_f(env));

            return rg.ToReader();
        }
    }

    static class ReaderMonadExtensions
    {
        internal static Reader<TEnv, TResult> ToReader<TEnv, TResult>(this Func<TEnv, TResult> f) =>
            Reader<TEnv, TResult>.From(f);
    }

    static class CurryExtensions
    {
        internal static Func<A, Func<B, TResult>> curried<A, B, TResult>(this Func<A, B, TResult> f) =>
            a => b => f(a, b);
    }

    public class ReaderMonadTest
    {
        string Greet(string name, Env env) =>
            $"Hi {name}! env={env}";

        [Fact]
        void run_the_monad()
        {
            Func<string, Env, string> greet = Greet;

            Func<string, Func<Env, string>> x =
                name
                    => env
                        => $"Hi {name}! env={env}";

            string CompletePlain(string name, Env env)
            {
                return $"Hi {name}! env={env}";
            }

            Reader<Env, string> Reader(Func<Env, string> f) =>
                f.ToReader();

            Reader<int, string> Complete(string name)
            {
                return Reader(env => $"Hi {name}! env={env}");
            }

            Reader<int, string> Monadic(string name)
            {
                Func<int, string> func = env => $"Hi {name}! env={env}";
                return func.ToReader();
            }

            Func<string, Env, string> complete = (name, env) =>
                $"Hi {name}! env={env}";

            var curried = complete.curried();
            var result = curried("Mario").ToReader().Run(42);
            result.Should().Be("Hi Mario! env=42");
        }

        [Fact]
        void binding_2_functions()
        {
            Reader<Env, string> Reader(Func<Env, string> f) =>
                f.ToReader();

            Reader<Env, string> First(string name) =>
                Reader(env =>
                    $"Hi {name}! env={env}");

            Reader<Env, string> Second(string s) =>
                Reader(env =>
                    env > 42 ? s.ToUpper() : s.ToLower());

            var gf = First("Mario").Bind(Second);

            gf.Run(42).Should().Be("hi mario! env=42");
        }

        [Fact]
        void map()
        {
            Reader<Env, string> Reader(Func<Env, string> f) =>
                f.ToReader();

            Reader<Env, string> First(string name) =>
                Reader(env =>
                    $"Hi {name}! env={env}");

            string Second(string s) =>
                s.ToUpper();

            var gf = First("Mario").Map(Second);

            gf.Run(42).Should().Be("HI MARIO! ENV=42");
        }
    }
}