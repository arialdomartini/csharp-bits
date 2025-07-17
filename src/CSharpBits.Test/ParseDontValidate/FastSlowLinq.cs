using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.ParseDontValidate;

internal class Child
{
    internal Guid ParentId { get; set; }
}

internal class Parent
{
    internal Guid Id { get; set; }
    internal List<Child> Children { get; set; } = [];
}

public class FastSlowLinq
{
    private const int NumberOfParents = 1_000_000;
    private const int ChildrenPerParent = 3;
    private readonly List<Parent> _parents;
    private readonly List<Child> _children;
    private readonly Random _random;

    private static List<Parent> V1(List<Parent> parents, List<Child> children)
    {
        var lookupChildren = children.ToLookup(child => child.ParentId);

        var dicChildren = new Dictionary<Guid, List<Child>>();
        foreach (Child child in children)
        {
            if (dicChildren.ContainsKey(child.ParentId) == false)
            {
                dicChildren.Add(child.ParentId, new List<Child> { child });
            }
            else
            {
                dicChildren[child.ParentId].Add(child);
            }
        }

        foreach (Parent parent in parents)
        {
            if (dicChildren.ContainsKey(parent.Id))
            {
                foreach (Child child in dicChildren[parent.Id])
                {
                    parent.Children.Add(child);
                }
            }
        }

        return parents;
    }

    private static List<Parent> V22(List<Parent> parents, List<Child> children)
    {
        var childrenLookup =
            children.Aggregate(
                seed: new Dictionary<Guid, List<Child>>(),
                (acc, child) =>
                {
                    if (!acc.TryGetValue(child.ParentId, out var list))
                    {
                        list = new List<Child>();
                        acc[child.ParentId] = list;
                    }

                    list.Add(child);
                    return acc;
                }
            );

        parents.ForEach(parent =>
        {
            if (childrenLookup.TryGetValue(parent.Id, out var childrenList))
            {
                parent.Children.AddRange(childrenList);
            }
        });


        return parents;


        // var details =
        //     children
        //         .GroupBy(x => x.ParentId)
        //         .ToDictionary(x => x.Key, x => x.ToList());

        // var details = children
        //     .ToLookup(x => x.ParentId)
        //     .ToDictionary(g => g.Key, g => g.ToList());

        // foreach (var nextTransaction in parents)
        //     if (details.GetValueOrDefault(nextTransaction.Id) is { } found)
        //         nextTransaction.Children.AddRange(found);
    }

    private static IEnumerable<Parent> V2(List<Parent> parents, List<Child> children) =>
        V2Select(parents, children);

    private static IEnumerable<Parent> V2Select(List<Parent> parents, List<Child> children)
    {
        var childLookup = children.ToLookup(c => c.ParentId);

        return parents
            .Select(parent =>
            {
                parent.Children.AddRange(childLookup[parent.Id]);
                return parent;
            });
    }

    private static IEnumerable<Parent> V2ForEach(List<Parent> parents, List<Child> children)
    {
        var childLookup = children.ToLookup(c => c.ParentId);

        parents
            .ForEach(parent =>
            {
                parent.Children.AddRange(childLookup[parent.Id]);
            });

        return parents;
    }

    private Parent CreateParent() => new() { Id = Guid.NewGuid() };

    private List<Parent> CreateParents() => Enumerable.Range(0, NumberOfParents).Map(i => CreateParent()).ToList();

    private Child CreateChild(Parent parent) =>
        new()
        {
            ParentId = parent.Id
        };

    private int UpTo(int max) => _random.Next(max) + 1;

    private List<Child> CreateChildrenFor(Parent parent) =>
        Enumerable.Range(0, UpTo(ChildrenPerParent)).Map(i => CreateChild(parent)).ToList();

    private IEnumerable<Child> CreateChildren(List<Parent> parents) => parents.Bind(CreateChildrenFor);

    public FastSlowLinq()
    {
        _random = new Random();
        _parents = CreateParents();
        _children = CreateChildren(_parents).ToList();
    }


    void Assertion(IEnumerable<Parent> parents)
    {
        parents.ForAll(p => p.Children.Count > 0);
    }

    [Fact]
    void test_V1()
    {
        var parents = V1(_parents, _children);

        Assert.Equal(NumberOfParents, parents.Count);
    }

    [Fact]
    void test_V2()
    {
        var parents = V2(_parents, _children);

        Assert.Equal(NumberOfParents, parents.Length());
    }

    [Fact]
    void test_V2Foreach()
    {
        var parents = V2ForEach(_parents, _children);

        Assert.Equal(NumberOfParents, parents.Length());
    }


    [Fact]
    private void Subs()
    {
        int a = 42;
        int b = 0;

        Assert.IsType<int>(a);
        Assert.IsType<int>(b);
        Assert.IsType<int>(a/b);
    }
}
