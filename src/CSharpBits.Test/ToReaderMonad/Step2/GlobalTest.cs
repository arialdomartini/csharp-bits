using System;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ToReaderMonad.Step2
{
    using Password = String;

    class Environment
    {
        internal Password Password { get; set; }
    }

    class A
    {
        private readonly B _b;

        internal A(B b)
        {
            _b = b;
        }

        internal string Greet(string name)
        {
            var greeting = $"Hi {name}";
            _b.Process(greeting);
            return greeting;
        }
    }

    class B
    {
        private readonly C _c;

        internal B(C c)
        {
            _c = c;
        }

        internal string Process(string s)
        {
            var ss = s.ToLower();
            return ss + _c.Third();
        }
    }

    class C
    {
        private readonly Environment _environment;

        public C(Environment environment)
        {
            _environment = environment;
        }

        internal string Third()
        {
            var password = _environment.Password;

            return $"C's result using Env={password}";
        }
    }

    public class GlobalTest
    {



        [Fact]
        void making_an_environment_parameter_via_Dependency_Injection()
        {
            // Wire components
            var environment = new Environment { Password = "some-password" };
            var c = new C(environment); // Inject Environment
            var b = new B(c);
            var a = new A(b);

            var result = a.Greet("Mario");

            result.Should().Be("A's result + B's result + C's result using Env=some-password");
        }
    }
}