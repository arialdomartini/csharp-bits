using System;
using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser
{
    internal static class StateAssertions
    {
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