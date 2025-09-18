using System;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal record Inner2 : IDisposable
{
    private readonly GlobalSingleton _globalSingleton;

    internal Inner2(GlobalSingleton globalSingleton)
    {
        _globalSingleton = globalSingleton;
        NestedScopesTest.Log("+ Inner2");
    }

    void IDisposable.Dispose() => NestedScopesTest.Log("- Inner2");

    internal void DoJob()
    {
        NestedScopesTest.Log("job Inner2");
        _globalSingleton.DoJob();
    }
}
