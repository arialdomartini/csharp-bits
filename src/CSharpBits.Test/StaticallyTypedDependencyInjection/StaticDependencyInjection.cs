using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

// ReSharper disable InconsistentNaming

namespace CSharpBits.Test.StaticallyTypedDependencyInjection;

internal interface IService
{
    internal static abstract int DoWork(string s);
}

internal interface Production : IService
{
    static int IService.DoWork(string s) => s.Length;
}

internal interface  Test : IService
{
    static int IService.DoWork(string s) => 42;
}

internal interface Client<_service> where _service : IService
{
    internal static int DoSomething(string s)
    {
        // Doing something and then invoking the injected Service
        return _service.DoWork(s);
    }
}

public class StaticDependencyInjection
{
    [Fact]
    private void use_production_implementation()
    {
        var length = Client<Production>.DoSomething("foo");

        Assert.Equal(3, length);
    }

    [Fact]
    private void use_test_implementation()
    {
        var length = Client<Test>.DoSomething("foo");

        Assert.Equal(42, length);
    }

    [Fact]
    void collection_of_interface()
    {
        List<Func<string, int>> calls =
            [
                Client<Production>.DoSomething,
                Client<Test>.DoSomething];

        Assert.Equivalent((List<int>)[3,42], calls.Select(c => c.Invoke("foo")));
    }
}
