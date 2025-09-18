using System;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal record GlobalSingleton : IDisposable
{
    internal GlobalSingleton() => NestedScopesTest.Log("+ GlobalSingleton");

    internal void DoJob() => NestedScopesTest.Log("job GlobalSingleton");

    void IDisposable.Dispose() => NestedScopesTest.Log("- GlobalSingleton");
}
