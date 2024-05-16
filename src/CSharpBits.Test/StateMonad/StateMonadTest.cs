using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using static CSharpBits.Test.StateMonad.LTree.LLeaf;
using static CSharpBits.Test.StateMonad.LTree.LNode;
using static CSharpBits.Test.StateMonad.Tree;
using static CSharpBits.Test.StateMonad.Tree.Leaf;
using static CSharpBits.Test.StateMonad.Tree.Node;
using static CSharpBits.Test.StateMonad.WithCounterExtensions;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace CSharpBits.Test.StateMonad;

interface Tree
{
    internal record Leaf(String Value) : Tree
    {
        internal static Tree leaf(String value) => new Leaf(value);
    }

    internal record Node(Tree Left, Tree Right) : Tree
    {
        internal static Tree node(Tree l, Tree r) => new Node(l, r);
    }
}

interface LTree
{
    internal record LLeaf(String Value, int Counter) : LTree
    {
        internal static LTree lLeaf(String value, int counter) => new LLeaf(value, counter);
    };

    internal record LNode(LTree Left, LTree Right) : LTree
    {
        internal static LTree lNode(LTree l, LTree r) => new LNode(l, r);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class StateMonadTes2
{
    private Tree TreeWith3Leaves =
        node(
            node(
                leaf("one"),
                leaf("two")),
            leaf("three"));

    private static LTree _lTree =
        lNode(
            lNode(
                lLeaf("one", 1),
                lLeaf("two", 2)),
            lLeaf("three", 3));


    [Fact]
    void relabel_tree_leaves()
    {
        Tree tree = TreeWith3Leaves;

        var relabled = Relabel(tree).Run(1).Item1;

        Assert.Equal(_lTree, relabled);
    }

    private WithCounter Relabel(Tree tree) =>
        tree switch
        {
            Leaf leaf => new WithCounter(counter => (lLeaf(leaf.Value, counter), counter + 1)),
            Node node => Recurse(node)
        };

    private WithCounter RecurseManual(Node node) =>
        new(counter =>
        {
            var (lLeft, counterL) = Relabel(node.Left).Run(counter);
            var (lRight, counterR) = Relabel(node.Right).Run(counterL);

            return (lNode(lLeft, lRight), counterR);
        });

    private WithCounter Recurse(Node node) =>
        Relabel(node.Left)
            .Then(leftBranch => Relabel(node.Right)
                .Then(rightBranch => 
                    Pure(leftBranch, rightBranch)));
}

static class WithCounterExtensions
{
    internal static WithCounter Then(this WithCounter a, Func<LTree, WithCounter> b) =>
        new(counter =>
        {
            var (aValue, aCounter) = a.Run(counter);
            var (bValue, bCounter) = b(aValue).Run(aCounter);

            return (bValue, bCounter);
        });

    internal static WithCounter Pure(LTree leftBranch, LTree rightBranch) => 
        new(c => (lNode(leftBranch, rightBranch), c));

    internal record WithCounter(Func<int, (LTree, int)> Run);
}
