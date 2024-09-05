using System;
using Xunit;
using static CSharpBits.Test.MonadsForTheRestOfsUs.WithCountExtensions;

namespace CSharpBits.Test.MonadsForTheRestOfsUs;

internal struct Unit
{
    internal static Unit Instance { get; } = new();
}

internal record WithCount<T>(Func<int, (T, int)> F);

public static class WithCountExtensions
{
    internal static WithCount<T> Pure<T>(T value) => new(count => (value, count));


    internal static (T, int) Run<T>(WithCount<T> value, int count) => value.F(count);

    internal static WithCount<TResult> Bind<T, TResult>(WithCount<T> a, Func<T, WithCount<TResult>> f) =>
        new(count =>
        {
            var (va, ca) = Run(a, count);
            var result = f(va);
            return Run(result, ca);
        });


    internal static WithCount<TResult> Select<T, TResult>(
        this WithCount<T> withCount,
        Func<T, TResult> selector) =>
        new(count =>
        {
            var (value, newCount) = withCount.F(count);
            return (selector(value), newCount);
        });

    internal static WithCount<TResult> SelectMany<T, TIntermediate, TResult>(
        this WithCount<T> withCount,
        Func<T, WithCount<TIntermediate>> intermediateSelector,
        Func<T, TIntermediate, TResult> resultSelector) =>
        new(count =>
        {
            var (value, intermediateCount) = withCount.F(count);
            var intermediateWithCount = intermediateSelector(value);
            var (intermediateValue, finalCount) = intermediateWithCount.F(intermediateCount);
            return (resultSelector(value, intermediateValue), finalCount);
        });

    internal static WithCount<T> Return<T>(T value) => new(count => (value, count));
}

public class MonadTest
{
    internal record Tree<T>
    {
        internal record Leaf(T Value) : Tree<T>;

        internal record Node(Tree<T> Left, Tree<T> Right) : Tree<T>;
    }

    private static readonly Tree<string> SomeTree = new Tree<string>.Node(
        new Tree<string>.Leaf("one"),
        new Tree<string>.Node(
            new Tree<string>.Leaf("two"),
            new Tree<string>.Leaf("three")
        )
    );


    private static Tree<T> BuildNode<T>(Tree<T> left, Tree<T> right) => new Tree<T>.Node(left, right);

    private static Tree<(T, int)> BuildLeaf<T>(T value, int count) => new Tree<(T, int)>.Leaf((value, count));


    private static WithCount<int> GetCount => new(c => (c, c));

    private static WithCount<Unit> PutCount(int c) => new(_ => (Unit.Instance, c));

    private static WithCount<Tree<(A, int)>> Index<A>(Tree<A> tree) =>
        tree switch
        {
            Tree<A>.Leaf(var v) => Bind(GetCount, c => Bind(PutCount(c + 1), _ => Pure(BuildLeaf(v, c)))),
            Tree<A>.Node(var l, var r) => Bind(Index(l), ll => Bind(Index(r), rr => Pure(BuildNode(ll, rr))))
        };

    private static WithCount<Tree<(A, int)>> IndexLINQ<A>(Tree<A> tree) =>
        tree switch
        {
            Tree<A>.Leaf(var v) =>
                from count in GetCount
                from _ in PutCount(count + 1)
                select BuildLeaf(v, count),
            
            Tree<A>.Node(var l, var r) =>
                from ll in Index(l)
                from rr in Index(r)
                select BuildNode(ll, rr)
        };

    [Fact]
    internal void IndexesATree()
    {
        var withCount = Index(SomeTree);
        var (indexed, _) = Run(withCount, 1);

        Assert.Equal(
            new Tree<(string, int)>.Node(
                new Tree<(string, int)>.Leaf(("one", 1)),
                new Tree<(string, int)>.Node(
                    new Tree<(string, int)>.Leaf(("two", 2)),
                    new Tree<(string, int)>.Leaf(("three", 3))
                )
            ),
            indexed
        );
    }
}
