using System;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal record Inner1(
    Inner2 Inner2) : IDisposable
{
    void IDisposable.Dispose() => NestedScopesTest.Log("- Inner1");

    internal void DoJob()
    {
        NestedScopesTest.Log("job Inner1");
        Inner2.DoJob();
    }
}
