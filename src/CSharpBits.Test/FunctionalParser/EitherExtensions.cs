using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser
{
    internal static class EitherExtensions
    {
        internal static bool IsRight<L, R>(this Either<L, R> either) => either.Match(l => false, r => true);
        internal static bool IsLeft<L, R>(this Either<L, R> either) => !either.IsRight();
    }
}