using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Equivalency;
using Xunit;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace CSharpBits.Test.StateMonad;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class StateMonadTes2
{
    [Fact]
    void count_the_leaves()
    {
        Tree<string> tree =
            new Tree<string>.Node(
                new Tree<string>.Node(
                    new Tree<string>.Leaf("one"),
                    new Tree<string>.Leaf("two")),
                new Tree<string>.Leaf("three"));

        var numberOfLeaves = CountLeaves(tree);

        Assert.Equal(3, numberOfLeaves);
    }

    private static int CountLeaves<t>(Tree<t> tree) =>
        tree switch
        {
            Tree<t>.Leaf => 1,
            Tree<t>.Node { Left: var left, Right: var right } => CountLeaves(left) + CountLeaves(right)
//          Node node => countLeaves(node.Left) + countLeaves(node.Right)
        };

    [Fact]
    void relabel_tree()
    {
        Tree<string> tree =
            new Tree<string>.Node(
                new Tree<string>.Node(
                    new Tree<string>.Leaf("one"),
                    new Tree<string>.Leaf("two")),
                new Tree<string>.Leaf("three"));

        var (labelled, count) = relabel(tree, 1);

        Tree<(int, string)> expected =
            new Tree<(int, string)>.Node(
                new Tree<(int, string)>.Node(
                    new Tree<(int, string)>.Leaf((1, "one")),
                    new Tree<(int, string)>.Leaf((2, "two"))),
                new Tree<(int, string)>.Leaf((3, "three")));

        Assert.Equal(expected, labelled);
        Assert.Equal(4, count);
    }

    (Tree<(int, string)>, int) relabel(Tree<string> tree, int counter)
    {
        return tree switch
        {
            Tree<string>.Leaf l => (new Tree<(int, string)>.Leaf((counter, l.Value)), counter + 1),
            Tree<string>.Node node => goInside(node, counter)
        };

        (Tree<(int, string)>, int) goInside(Tree<string>.Node node, int i0)
        {
            var (relabeledLeft, i1) = relabel(node.Left, i0);
            var (relabeledRght, i2) = relabel(node.Right, i1);
            return
                (new Tree<(int, string)>.Node(Left: relabeledLeft, Right: relabeledRght), i2);
        }
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal interface Tree<t>
{
    internal record Leaf(t Value) : Tree<t>;

    internal record Node(Tree<t> Left, Tree<t> Right) : Tree<t>;
}
