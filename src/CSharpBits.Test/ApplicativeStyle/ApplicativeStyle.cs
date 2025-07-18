using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static CSharpBits.Test.ApplicativeStyle.Person;
using static CSharpBits.Test.ApplicativeStyle.ResultInstances;

namespace CSharpBits.Test.ApplicativeStyle;

record Manager(string Name);

record Person(string Name, int Age, Manager Manager)
{
    internal static Func<string, Func<int, Func<Manager, Person>>> Build => name => age => manager => new(name, age, manager);
}

abstract record Result<V>
{
    internal abstract V GetValue();
}

internal record Failure<V>(List<string> Errors) : Result<V>
{
    internal override V GetValue() => throw new ApplicationException("Not a Success");
}

internal record Success<V>(
    V Value) : Result<V>
{
    internal override V GetValue() => Value;
}

internal static class ResultInstances
{
    internal static Result<A> Point<A>(A a) => new Success<A>(a);

    internal static Success<Func<A, Func<B, Func<C, D>>>> Apply<A,B,C,D>(this Func<A,B,C,D> f) =>
        new(a => b => c => f(a, b, c));

    internal static Result<B> Ap<A, B>(Result<Func<A, B>> fr, Result<A> ar) =>
        fr switch
        {
            Failure<Func<A, B>> error =>
                ar switch
                {
                    Failure<A> failure => new Failure<B>(error.Errors.Append(failure.Errors).ToList()),
                    Success<A> => new Failure<B>(error.Errors),
                },


            Success<Func<A, B>> f =>
                ar switch
                {
                    Failure<A> error => new Failure<B>(error.Errors),
                    Success<A> a => new Success<B>(f.Value(a.Value))
                }
        };

    internal static Result<B> With<A, B>(this Result<Func<A, B>> fr, Result<A> ar) => Ap(fr, ar);

    private static Result<B> Map<A,B>(Func<A, B> f, Result<A> a) =>
        a switch
        {
            Success<A> success => new Success<B>(f(success.Value)),
            Failure<A> failure => new Failure<B>(failure.Errors)
        };

    private static Result<B> Bind<A, B>(Func<A, Result<B>> f, Result<A> a) =>
        a switch
        {
            Success<A> success => f(success.Value),
            Failure<A> failure => new Failure<B>(failure.Errors)
        };

    private static Result<B> Select<A, B>(this Result<A> a, Func<A, B> f) =>
        Map(f, a);

    private static Result<B> SelectMany<A, B>(this Result<A> a, Func<A, Result<B>> f) =>
        Bind(f, a);

    public static Result<B> SelectMany<A, X, B>(
        this Result<A> a,
        Func<A, Result<X>> collectionSelector,
        Func<A, X, B> resultSelector) =>
        a.SelectMany(
            x => collectionSelector(x).Select(
                y => resultSelector(x, y)
            )
        );
}

// Some tests
public class ApplicativeStyle
{
    private Manager Maria = new Manager(Name: "Maria");

    [Fact]
    void successfully_build_a_person()
    {
        Result<string> GetName() => new Success<string>("Shansai");
        Result<int> GetAge() => new Success<int>(99);
        Result<Manager> GetManager() => new Success<Manager>(Maria);

        var person = Ap(Ap(Ap(
            Point(Build),
            GetName()),
            GetAge()),
            GetManager());

        Assert.Equal(new Person("Shansai", 99, Maria), person.GetValue());
    }


    [Fact]
    void error_case()
    {
        Result<string> GetName() => new Success<string>("Shansai");
        Result<int> GetAge() => new Failure<int>(["too old!"]);
        Result<Manager> GetManager() => new Failure<Manager>(["no one wants to work with him!"]);

        var result =
            Ap(Ap(Ap(Point(
                Build),
                GetName()),
                GetAge()),
                GetManager());

        Assert.IsType<Failure<Person>>(result);

        List<string> expectedErrors = ["too old!", "no one wants to work with him!"];

        Assert.Equivalent(expectedErrors, ((Failure<Person>)result).Errors);
    }

    [Fact]
    void error_case_better_syntax()
    {
        // example of API calls. Some of them would fail
        Result<string> GetName() => new Success<string>("Shansai");
        Result<int> GetAge() => new Failure<int>(["too old!"]);
        Result<Manager> GetManager() => new Failure<Manager>(["no one wants to work with him!"]);

        // factory for a person
        Func<string, int, Manager, Person> buildPerson = (name, age, manager) => new Person(name, age, manager);

        // Invocation in the Applicative Sytle
        var result =
            buildPerson.Apply()
                .With(GetName())
                .With(GetAge())
                .With(GetManager());


        // when it fails, the collection of errors is returned
        Assert.IsType<Failure<Person>>(result);

        List<string> expectedErrors = ["too old!", "no one wants to work with him!"];

        Assert.Equivalent(expectedErrors, ((Failure<Person>)result).Errors);
    }

    [Fact]
    void error_case_with_LINQ_would_capture_first_error_only()
    {
        Result<string> GetName() => new Success<string>("Shansai");
        Result<int> GetAge() => new Failure<int>(["too old!"]);
        Result<Manager> GetManager() => new Failure<Manager>(["no one wants to work with him!"]);


        var result =
            from name in GetName()
            from age in GetAge()
            from manager in GetManager()
            select new Person(name, age, manager);

        Assert.IsType<Failure<Person>>(result);

        List<string> expectedErrors = ["too old!"];

        Assert.Equivalent(expectedErrors, ((Failure<Person>)result).Errors);
    }
}
