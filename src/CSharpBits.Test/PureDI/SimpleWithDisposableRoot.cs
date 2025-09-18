using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.PureDI;

file class Scope : IDisposable
{
    private List<IDisposable> _disposables;

    internal Level1 Create()
    {
        var level3 = new Level3();
        var level2 = new Level2(level3);
        var level1 = new Level1(level2);

        _disposables = [level1, level2, level3];
        return level1;
    }

    public void Dispose()
    {
        _disposables.ForEach(s => s.Dispose());
    }
}

file record  Level1(
    Level2 Level2) : IDisposable
{
    internal bool HasBeenDisposedOf;

    void IDisposable.Dispose()
    {
        HasBeenDisposedOf = true;
    }
}

file record  Level2(
    Level3 Level3) : IDisposable
{
    internal bool HasBeenDisposedOf;

    void IDisposable.Dispose()
    {
        HasBeenDisposedOf = true;
    }
}

file record Level3 : IDisposable
{
    internal bool HasBeenDisposedOf;

    void IDisposable.Dispose()
    {
        HasBeenDisposedOf = true;
    }
}

public class SimpleCompositionRootWithDisposableRoot
{
    [Fact]
    void chain_of_objects()
    {
        var compositionRoot = new Scope();
        var level1 = compositionRoot.Create();

        compositionRoot.Dispose();

        Assert.True(level1.HasBeenDisposedOf);
        Assert.True(level1.Level2.HasBeenDisposedOf);
        Assert.True(level1.Level2.Level3.HasBeenDisposedOf);
    }
}
