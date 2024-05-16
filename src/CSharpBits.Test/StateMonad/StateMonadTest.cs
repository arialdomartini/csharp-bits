using System;
using Xunit;
using static CSharpBits.Test.StateMonad.Factories;
using static CSharpBits.Test.StateMonad.GTree<string>;
using static CSharpBits.Test.StateMonad.Tree;

namespace CSharpBits.Test.StateMonad;

public class StateMonadTest
{
    [Fact]
    void count_leaves()
    {
        var tree =
            node(
                node(
                    leaf("one"),
                    leaf("two")),
                leaf("three"));

        var numberOfLeaves = CountLeaves(tree: tree);

        Assert.Equal(3, numberOfLeaves);
    }

    private int CountLeaves(Tree tree) =>
        tree switch
        {
            Leaf leaf => 1,
            Node node => CountLeaves(node.Left) + CountLeaves(node.Right)
        };

    [Fact]
    void map_leaves_tree_as_a_functor()
    {
        var tree =
            gnode(
                gnode(
                    gleaf("one"),
                    gleaf("two")),
                gleaf("three"));


        var mapped = MapLeaves(tree, s => s.Length());

        var expected =
            gnode(
                gnode(
                    gleaf(3),
                    gleaf(3)),
                gleaf(5));

        Assert.Equal(expected, mapped);
    }

    [Fact]
    void index_a_tree()
    {
        var tree =
            node(
                node(
                    leaf("one"),
                    leaf("two")),
                leaf("three"));


        ITree indexed = IndexLeaves(tree).Run(1).Item1;

        var expected =
            inode(
                inode(
                    ileaf("one", 1),
                    ileaf("two", 2)),
                ileaf("three", 3));

        Assert.Equal(expected, indexed);
    }

    private static WithCounter IndexLeaves(Tree tree) => 
        tree switch
        {
            Leaf leaf => new(counter =>(ileaf(leaf.Value, counter), counter + 1)),
            Node node => Recurse(node)
        };

    private static WithCounter Recurse(Node node)
    {
        return new(counter =>
        {
            WithCounter indexLeaves = IndexLeaves(node.Left);
            var (iLeft, counterLeft) = indexLeaves.Run(counter);
            var (iRight, counterRight) = IndexLeaves(node.Right).Run(counterLeft);
        
            return (inode(iLeft, iRight), counterRight);
        });
    }

    private GTree<int> MapLeaves(GTree<string> tree, Func<string, int> func) =>
        tree switch
        {
            GLeaf leaf => gleaf(func(leaf.Value)),
            GNode node => gnode(MapLeaves(node.Left, func), MapLeaves(node.Right, func))
        };
}

record WithCounter(Func<int, (ITree, int)> Run);

// data Tree = Leaf | Node Tree Tree
internal interface Tree
{
    internal record Leaf(string Value) : Tree;

    internal record Node(Tree Left, Tree Right) : Tree;
}

internal interface ITree
{
    internal record ILeaf(string Value, int Index) : ITree;

    internal record INode(ITree Left, ITree Right) : ITree;
}

// data GTree a = Leaf a | Node (GTree a) (GTree a)
internal interface GTree<T>
{
    internal record GLeaf(T Value) : GTree<T>;

    internal record GNode(GTree<T> Left, GTree<T> Right) : GTree<T>;
}

static class Factories
{
    internal static Tree leaf(string value) => new Leaf(value);
    internal static Tree node(Tree left, Tree right) => new Node(left, right);

    internal static ITree ileaf(string value, int index) => new ITree.ILeaf(value, index);
    internal static ITree inode(ITree left, ITree right) => new ITree.INode(left, right);

    internal static GTree<T> gleaf<T>(T value) => new GTree<T>.GLeaf(value);
    internal static GTree<T> gnode<T>(GTree<T> left, GTree<T> right) => new GTree<T>.GNode(left, right);
}
