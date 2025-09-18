using Xunit;

namespace CSharpBits.Test.PureDI;

file class CompositionRoot
{
    internal Level1 Create()
    {
        var level3 = new Level3();
        var level2 = new Level2(level3);
        return new(level2);
    }
}

file class Level1(
    Level2 level2);

file class Level2(
    Level3 level3);

file class Level3;

public class Simple
{
    [Fact]
    void chain_of_objects()
    {
        var compositionRoot = new CompositionRoot();

        var level1 = compositionRoot.Create();

        Assert.NotNull(level1);
    }
}
