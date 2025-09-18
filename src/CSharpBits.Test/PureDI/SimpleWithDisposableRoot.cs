using System;
using Xunit;

namespace CSharpBits.Test.PureDI;

file class Scope : IDisposable
{
    private IDisposable _level1;

    internal Level1 Create()
    {
        var level3 = new Level3();
        var level2 = new Level2(level3);
        var level1 = new Level1(level2);

        _level1 = level1;
        return level1;
    }

    public void Dispose()
    {
        _level1.Dispose();
    }
}

file class Level1(
    Level2 level2) : IDisposable
{
    internal bool HasBeenDisposedOf;

    void IDisposable.Dispose()
    {
        HasBeenDisposedOf = true;
    }
}

file class Level2(
    Level3 level3);

file class Level3;

public class SimpleCompositionRootWithDisposableRoot
{
    [Fact]
    void chain_of_objects()
    {
        var compositionRoot = new Scope();
        var level1 = compositionRoot.Create();

        compositionRoot.Dispose();

        Assert.True(level1.HasBeenDisposedOf);
    }
}
