using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser;

internal static class EitherExtensions
{
    internal static bool IsRight<TL, TR>(this Either<TL, TR> either) => either.Match(l => false, r => true);
    internal static bool IsLeft<TL, TR>(this Either<TL, TR> either) => !either.IsRight();
}