using System;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal class ScopeSingleton : IDisposable
{
    internal ScopeSingleton() => NestedScopesTest.Log("+ ScopeSingleton");

    void IDisposable.Dispose() => NestedScopesTest.Log("- ScopeSingleton");
}