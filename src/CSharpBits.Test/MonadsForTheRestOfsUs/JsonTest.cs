using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Text.Json;

namespace CSharpBits.Test.MonadsForTheRestOfsUs;

public class JsonTest
{
    public record Person(string Id, string Name, string Email);

    [Fact]
    void person()
    {
        var json = "[{\"id\":\"\", \"name\":\"\"}]";

        var people = JsonSerializer.Deserialize<List<Person>>(json);

        Assert.Single(people);

        var emptyPerson = new Person(
            Id: null,
            Name: null,
            Email: null);

        var deserializedPerson = people[0];
        Assert.Equal(emptyPerson, deserializedPerson);
    }

    [Fact]
    void division_by_zero()
    {
        void badbad(out int i)
        {
            i = 2;
        }

        int myValueWillChange = 1;
        Assert.Equal(1, myValueWillChange);

        badbad(out myValueWillChange);
        Assert.Equal(2, myValueWillChange);
    }

    [Fact]
    void filter_and_count()
    {
        var righe = Righe();
        var pari = righe.Where(i => i is "2" or "4");

        var righeC = righe.Select((e, i) => (e, i));
        var pariC = righeC.Where(t =>
        {
            var (e, i) = t;
            return e is "2" or "4";
        }).Select((e, i) => (e, i));

        IEnumerable<((string e, int i), int)> valueTuples = righeC.WhereI(e => e.e == "4");
    }

    (int, IEnumerable<int>) empty = (0, []);

    private IEnumerable<string> Righe()
    {
        yield return "1";
        yield return "2";
        yield return "3";
        yield return "4";
    }
}

internal static class WithCountExt
{
    internal static IEnumerable<(T, int)> Index<T>(this IEnumerable<T> xs) => xs.Select((e, i) => (e, i));
    internal static IEnumerable<(T, int)> WhereI<T>(this IEnumerable<T> xs, Func<T, bool> f) => xs.Where((e, i) => f(e)).Index();
}
