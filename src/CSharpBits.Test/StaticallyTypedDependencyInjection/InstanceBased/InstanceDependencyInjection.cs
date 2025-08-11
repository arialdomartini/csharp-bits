using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Xunit;

// ReSharper disable InconsistentNaming

namespace CSharpBits.Test.StaticallyTypedDependencyInjection.InstanceBased;

internal interface ServiceB
{
    internal static abstract int DoWorkB(string s);
}

internal interface ProductionB : ServiceB
{
    static int ServiceB.DoWorkB(string s) => s.GetHashCode();
}



internal interface ServiceA
{
    internal static abstract int DoWorkA(string s);
}

internal interface ProductionA<_serviceB> : ServiceA where _serviceB : ServiceB
{
    static int ServiceA.DoWorkA(string s) => s.Length + _serviceB.DoWorkB(s);
}

internal interface  TestA : ServiceA
{
    static int ServiceA.DoWorkA(string s) => 42;
}

internal class Client<_service> where _service : ServiceA
{
    internal int DoSomething(string s)
    {
        // Do something, then invoke the injected Service
        return _service.DoWorkA(s);
    }
}

public class StaticDependencyInjection
{
    [Fact]
    private void use_production_implementation()
    {
        var client = new Client<ProductionA<ProductionB>>();

        var length = client.DoSomething("foo");

        Assert.Equal(3, length);
    }

    [Fact]
    private void use_test_implementation()
    {
        var client = new Client<TestA>();

        var length = client.DoSomething("foo");

        Assert.Equal(42, length);
    }

    [Fact]
    void collection_of_interface()
    {
        List<Func<string, int>> calls =
            [
                new Client<ProductionA<ProductionB>>().DoSomething,
                new Client<TestA>().DoSomething];

        Assert.Equivalent((List<int>)[3,42], calls.Select(c => c.Invoke("foo")));
    }
}
