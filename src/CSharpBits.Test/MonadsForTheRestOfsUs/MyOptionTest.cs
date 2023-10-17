using System;
using Xunit;
using static CSharpBits.Test.MonadsForTheRestOfsUs.MyOption.Maybe<string>;

namespace CSharpBits.Test.MonadsForTheRestOfsUs.MyOption;

abstract record Maybe<A>
{
    internal record Just(A Value) : Maybe<A>;
    internal record Nothing : Maybe<A>;

    public B Run<B>(Func<A, B> just, Func<B> nothing) =>
        this switch
        {
            Just a => just(a.Value),
            Nothing => nothing()
        };
}

public class MyOptionTest
{
    Maybe<A> Return<A>(A a) => new Maybe<A>.Just(a);

    Maybe<B> Bind<A, B>(Func<A, Maybe<B>> f, Maybe<A> a)
    {
        return a.Run(
            just: a => f(a),
            nothing: () => new Maybe<B>.Nothing());
    }
    
    [Fact]
    void maybe_return()
    {
        int i = 42;
        Maybe<int> maybeI = Return(42);

        var value = maybeI.Run(
            just: a => $"I got a {a}", 
            nothing: () => "I got nothing!");

        Assert.Equal("I got a 42", value);
    }
    
    [Fact]
    void bind_just_case()
    {
        Maybe<string> ReturnsSomething(int a) => 
            new Just($"I'm {a}, I feel so young!");

        Maybe<string> something = Bind(ReturnsSomething, Return(42));
        var value = something.Run(
            just: b => b,
            nothing: () => "No result, sorry");
        
        Assert.Equal("I'm 42, I feel so young!", value);
    }

    [Fact]
    void bind_nothing_case()
    {
        Maybe<string> ReturnsNothing(int a) => 
            new Nothing();

        Maybe<string> something = Bind(ReturnsNothing, Return(42));
        var value = something.Run(
            just: b => b,
            nothing: () => "No result, sorry");
        
        Assert.Equal("No result, sorry", value);
    }
}
