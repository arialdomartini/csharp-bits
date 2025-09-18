using System;
using System.Collections.Generic;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal class Scope : IDisposable
{
    private List<IDisposable> _disposables;

    internal WithNestedScopes.Level1 Create()
    {
        var globalSingleton = new GlobalSingleton();

        Func<InnerScope> innerScopeFactory = () => new(globalSingleton);


        var level1 = new WithNestedScopes.Level1(innerScopeFactory, globalSingleton);

        _disposables = [level1, globalSingleton];

        return level1;
    }

    public void Dispose()
    {
        _disposables.DisposeOfAll();
    }
}
