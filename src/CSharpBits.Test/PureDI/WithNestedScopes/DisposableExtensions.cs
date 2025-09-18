using System;
using System.Collections.Generic;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

internal static class DisposableExtensions
{
    internal static void DisposeOfAll(this List<IDisposable> disposables) => disposables.ForEach(s => s.Dispose());
}