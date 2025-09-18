using System;
using System.Collections.Generic;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal class InnerScope(GlobalSingleton globalSingleton) : IDisposable
{
    private List<IDisposable> _disposables;

    internal Inner1 Create()
    {
        var scopeSingleton = new ScopeSingleton();
        var inner2 = new Inner2(globalSingleton);
        var inner1 = new Inner1(inner2);

        _disposables = [inner1, inner2, scopeSingleton];

        return inner1;
    }

    void IDisposable.Dispose()
    {
        _disposables.DisposeOfAll();
    }
}
