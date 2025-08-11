using Xunit;

namespace CSharpBits.Test.StaticallyTypedDependencyInjection.Shansai;

interface IService
{
    static abstract int CalculateLength(string s);
}

public class Production : IService
{
    static int IService.CalculateLength(string s) => s.Length;
}

public class Test : IService
{
    static int IService.CalculateLength(string s) => 42;
}

internal static class Client<TService> where TService : IService
{
    internal static int DoSomething(string s) => TService.CalculateLength(s);
}


public class ClientTest
{
    [Fact]
    void production_Test()
    {
        var l = Client<Production>.DoSomething("foo");

        Assert.Equal(3, l);
    }

    [Fact]
    void test_Test()
    {
        var l = Client<Test>.DoSomething("foo");

        Assert.Equal(42, l);
    }
}
