using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace CSharpBits.Test.StateMonad;

interface Tree<T>
{
    internal record Leaf(T Value) : Tree<T>;

    internal record Node(Tree<T> Left, Tree<T> Right) : Tree<T>;
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class StateMonadTes2
{
    private Tree<string>.Node TreeWith3Leaves =
        new(
            new Tree<string>.Node(
                new Tree<string>.Leaf("one"),
                new Tree<string>.Leaf("two")),
            new Tree<string>.Leaf("three"));

    private static Tree<(int, string)>.Node LabeledTree = new Tree<(int, string)>.Node(
        new Tree<(int, string)>.Node(
            new Tree<(int, string)>.Leaf((1, "one")),
            new Tree<(int, string)>.Leaf((2, "two"))),
        new Tree<(int, string)>.Leaf((3, "three")));

    [Fact]
    void count_the_leaves()
    {
        Tree<string> tree =
            TreeWith3Leaves;

        var numberOfLeaves = CountLeaves(tree);

        Assert.Equal(3, numberOfLeaves);
    }

    [Fact]
    void map_leaves()
    {
        Tree<string> tree = TreeWith3Leaves;

        var mapped = Map<string, string>(x => x)(tree);

        Tree<string> expectedTree = TreeWith3Leaves;

        Assert.Equal(expectedTree, mapped);
    }

    [Fact]
    void relabel_tree_leaves()
    {
        Tree<string> tree = TreeWith3Leaves;

        var relabled = Relabel(tree).Run(1).Item1;

        Assert.Equal(LabeledTree, relabled);
    }


    private WithCounter<Tree<(int, string)>> Relabel(Tree<string> tree)
    {
        if (tree is Tree<string>.Leaf leaf)
        {
            return new WithCounter<Tree<(int, string)>>(counter =>
                (new Tree<(int, string)>.Leaf((counter, leaf.Value)), counter + 1));
        }

        if (tree is Tree<string>.Node node)
        {
            return
                Relabel(node.Left)
                    .Then(relabeledLeft => Relabel(node.Right)
                        .Then(relabeledRight =>
                            new WithCounter<Tree<(int, string)>>(
                                counter => (new Tree<(int, string)>.Node(relabeledLeft, relabeledRight), counter))));
        }

        throw new NotImplementedException();
    }

    private Func<Tree<A>, Tree<B>> Map<A, B>(Func<A, B> func) =>
        tree => tree switch
        {
            Tree<A>.Leaf l => new Tree<B>.Leaf(func(l.Value)),
            Tree<A>.Node l => new Tree<B>.Node(
                Left: Map(func)(l.Left),
                Right: Map(func)(l.Right))
        };

    private int CountLeaves<T>(Tree<T> tree) =>
        tree switch
        {
            Tree<T>.Leaf => 1,
            Tree<T>.Node node => CountLeaves(node.Left) + CountLeaves(node.Right)
        };
}

internal record WithCounter<T>(Func<int, (T, int)> Run);

static class WithCounterExtensions
{
    internal static WithCounter<B> Then<A, B>(this WithCounter<A> f, Func<A, WithCounter<B>> g)
    {
        return new WithCounter<B>(counter =>
        {
            var (resultA, counter1) = f.Run(counter);
            var (resultB, counter2) = g(resultA).Run(counter1);
            return (resultB, counter2);
        });
    }
}
