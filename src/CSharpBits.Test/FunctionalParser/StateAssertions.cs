using System;
using FluentAssertions;
using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser
{
    internal static class StateAssertions
    {
        internal static void ShouldBeRight<TLeft, TRight>(this Either<TLeft, TRight> either) => either.IsRight().Should().Be(true);
        internal static void ShouldBeLeft<TLeft, TRight>(this Either<TLeft, TRight> either) => either.IsLeft().Should().Be(true);

        internal static TRight Right<TLeft, TRight>(this Either<TLeft, TRight> either) =>
            either.Match(
                l => throw new Exception("Either expected to be right is actually left"),
                r => r);

        internal static TLeft Left<TLeft, TRight>(this Either<TLeft, TRight> either) =>
            either.Match(
                l => l,
                r => throw new Exception("Either expected to be left is actually right"));
    }
}