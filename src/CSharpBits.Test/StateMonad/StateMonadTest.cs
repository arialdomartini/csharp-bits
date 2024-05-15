using System.Diagnostics.CodeAnalysis;
using Xunit;
using static CSharpBits.Test.StateMonad.Tree;
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace CSharpBits.Test.StateMonad;

public class StateMonadTes2
{
    [Fact]
    void count_the_leaves()
    {
        Tree tree =
            new Node(
                new Node(
                    new Leaf("one"),
                    new Leaf("two")),
                new Leaf("three"));
       
        var numberOfLeaves = CountLeaves(tree);

        Assert.Equal(3, numberOfLeaves);
    }

    private static int CountLeaves(Tree tree) =>
        tree switch
        {
            Leaf => 1,
            Node { Left: var left, Right: var right } => CountLeaves(left) + CountLeaves(right)
//          Node node => countLeaves(node.Left) + countLeaves(node.Right)
        };
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal interface Tree
{
    internal record Leaf(string Value) : Tree;

    internal record Node(Tree Left, Tree Right) : Tree;
}
