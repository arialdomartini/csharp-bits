﻿using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Equivalency;
using Xunit;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace CSharpBits.Test.StateMonad;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class StateMonadTes2
{
    private Tree<string>.Node TreeWith3Leaves = new Tree<string>.Node(
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
        Tree<string> tree = TreeWith3Leaves;

        var (labelled, count) = relabel(tree)(1);

        Assert.Equal(LabeledTree, labelled);
        Assert.Equal(4, count);
    }
    
    Func<int, (Tree<(int, string)>, int)> relabel(Tree<string> tree) =>
        counter =>
        {
            return tree switch
            {
                Tree<string>.Leaf l => (new Tree<(int, string)>.Leaf((counter, l.Value)), counter + 1),
                Tree<string>.Node node => RelableRecursively(node, counter)
            };
        };

    (Tree<(int, string)>, int) RelableRecursively(Tree<string>.Node node, int counter)
    {
        var (relabeledLeft, counterLeft) = relabel(node.Left)(counter);
        var (relabeledRight, counterRight) = relabel(node.Right)(counterLeft);
        return
            (new Tree<(int, string)>.Node(Left: relabeledLeft, Right: relabeledRight), counterRight);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal interface Tree<t>
{
    internal record Leaf(t Value) : Tree<t>;

    internal record Node(Tree<t> Left, Tree<t> Right) : Tree<t>;
}
