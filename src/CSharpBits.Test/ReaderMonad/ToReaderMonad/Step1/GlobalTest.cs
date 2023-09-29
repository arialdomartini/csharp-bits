using System;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.ReaderMonad.ToReaderMonad.Step1;

using Password = String;

static class Environment
{
    internal static Password Password { get; set; }
}

class A
{
    internal string DoSomething()
    {
        var bResult = new B().Second();
        return $"A's result + {bResult}";
    }
}

class B
{
    internal string Second()
    {
        var cResult = new C().Third();
        return $"B's result + {cResult}";
    }
}
class C
{
    internal string Third()
    {
        var env = Environment.Password;
        return $"C's result using Env={env}";
    }
}

public class GlobalTest
{
    [Fact]
    void making_an_environment_parameter_available_via_a_static_global()
    {
        Environment.Password = "some-password";

        var result = new A().DoSomething();

        result.Should().Be("A's result + B's result + C's result using Env=some-password");
    }
}