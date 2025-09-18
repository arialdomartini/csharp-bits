using System;
using System.Linq;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal record Level1 : IDisposable
{
    private readonly Func<InnerScope> _innerScopeFactory;
    private readonly GlobalSingleton _globalSingleton;

    public Level1(Func<InnerScope> innerScopeFactory, GlobalSingleton globalSingleton)
    {
        NestedScopesTest.Log("new Level1");
        _innerScopeFactory = innerScopeFactory;
        _globalSingleton = globalSingleton;
    }

    void IDisposable.Dispose()
    {
        NestedScopesTest.Log("Disposing of Level1");
    }

    internal void DoJob()
    {
        Enumerable.Range(1, 3).ToList().ForEach(_ =>
            {
                using var scope = _innerScopeFactory();
                var inner1 = scope.Create();
                _globalSingleton.DoJob();
                inner1.DoJob();
            }
        );
    }
}
