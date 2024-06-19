using System;
using System.Collections;
using CSharpBits.Test.ReaderMonad.ToReaderMonad.Step1;
using Xunit;

namespace CSharpBits.Test.Applicative;

record Person(string Name, int Age);

abstract record Either<L, R>
{
    internal record Left(L L) : Either<L, R>;

    internal record Right(R R) : Either<L, R>;
}

static class EitherFuncs
{
    // (a -> b) -> Either l a -> Either l b
    internal static Either<l, b> map<l, a, b>(this Func<a, b> f, Either<l, a> ela) =>
        ela switch
        {
            Either<l, a>.Left(var lv) => new Either<l, b>.Left(lv),
            Either<l, a>.Right(var r) => new Either<l, b>.Right(f(r))
        };

    // Either l (a -> b) -> Either l a -> Either l b
    internal static Either<string, b> ap<a, b>(this Either<string, Func<a, b>> ef, Either<string, a> ela)=>
        ef switch
        {
            Either<string, Func<a, b>>.Left(var lv) => 
                ela switch
                {
                    Either<string, a>.Left(var lvv) => new Either<string, b>.Left(lv + lvv),
                    Either<string, a>.Right(var _) => new Either<string, b>.Left(lv)
                },

            Either<string, Func<a, b>>.Right(var f) =>
                ela switch
                {
                    Either<string, a>.Left(var lvv) => new Either<string, b>.Left(lvv),
                    Either<string, a>.Right(var r) => new Either<string, b>.Right(f(r))
                }
        };

    internal static Either<l, r> pure<l, r>(this r v) => new Either<l, r>.Right(v);
    internal static Func<a, Func<b, c>> curried<a, b, c>(this Func<a, b, c> f) => a => b => f(a, b);
    internal static Func<a, Func<b, Func<c, d>>> curried<a, b, c, d>(this Func<a, b, c, d> f) => a => b => c => f(a, b, c);
}

public class ApplicativeParsingTest
{
    Either<string, string> parseName() => new Either<string, string>.Right("Shansai");
    Either<string, int> parseAge() => new Either<string, int>.Right(87);

    Func<string, int, Person> buildPerson => (name, age) => new Person(name, age);

    [Fact]
    void builds_person()
    {
        string name = "Shansai";
        int age = 87;
        Person person = buildPerson(name, age);
    }
    
    [Fact]
    void builds_person_in_applicative_way()
    {
        
        Either<string, string> name = parseName();
        Either<string, int> age = parseAge();

        // Person person = buildPerson              (name,    age);
        
        var either =       buildPerson.curried().map(name).ap(age);
        
        
        //                 buildPerson <$> name <*> age
        //            pure buildPerson <*> name <*> age
        //              [| buildPerson         name     age |]
        //                 pre buildPerson     name     age
    }
}
