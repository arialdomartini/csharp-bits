using System;

namespace CSharpBits.Test.ParseDontValidate.Parse;

#pragma warning disable CS8509
internal static class ResultExtensions
{
    internal static Result<B> map<A, B>(Func<A, B> f, Result<A> result) =>
        result switch
        {
            Success<A> success => Result<B>.Success(f(success.Value)),
            Error<A> error => Result<B>.Error(error.ErrorMessage)
        };

    internal static Result<A> pure<A>(A value) => Result<A>.Success(value);

    internal static Result<B> ap<A, B>(Result<Func<A, B>> f, Result<A> a) =>
        (a, f) switch
        {
            (Error<A> ae, Error<Func<A, B>> fe) fError => Result<B>.Error($"{ae.ErrorMessage}\n{fe.ErrorMessage}"),
            (_, Error<Func<A, B>> fe) fError => Result<B>.Error(fe.ErrorMessage),
            (Error<A> ae, _) fError => Result<B>.Error(ae.ErrorMessage),
            (Success<A> aa, Success<Func<A, B>> ff) => Result<B>.Success(ff.Value(aa.Value))
        };

    internal static Result<B> apply<A, B>(this Result<Func<A, B>> f, Result<A> a) => ap(f, a);

    internal static Result<B> SelectMany<A, TIntermediate, B>(
        this Result<A> result,
        Func<A, Result<TIntermediate>> intermediateSelector,
        Func<A, TIntermediate, B> resultSelector) =>
        result switch
        {
            Error<A> error => Result<B>.Error(error.ErrorMessage),
            Success<A> rr => intermediateSelector(rr.Value) switch
            {
                Error<TIntermediate> error => Result<B>.Error(error.ErrorMessage),
                Success<TIntermediate> ii => Result<B>.Success(resultSelector(rr.Value, ii.Value))
            },
        };

    internal static Func<A, Func<B, Func<C, D>>> curry<A, B, C, D>(Func<A, B, C, D> f) => a => b => c => f(a, b, c);
}
