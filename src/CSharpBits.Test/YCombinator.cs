using FsCheck;
using FsCheck.Xunit;
using static FsCheck.Prop;

namespace CSharpBits.Test
{
    public class YCombinator
    {
        private readonly Arbitrary<int> PositiveNumbers = Arb.From(Gen.Choose(0, 8_000));

        private delegate int Sum(int n);
        private delegate Sum MkSum(MkSum mkSum);

        private static readonly MkSum mkSum =
            self =>
            {
                // This will not work in C#, because it's not a lazy language 
                // var f = self(self);
                // We have to make it lazy inhibiting the eager evaluation with a lambda
                Sum f = x => self(self)(x);
                return n => 
                    n == 0 ? 0 : n + f(n - 1);
            };

        private static readonly Sum sum = 
            n => 
                mkSum(mkSum)(n);
        
        
        [Property]
        Property it_meets_the_gauss_formula() =>
            ForAll(PositiveNumbers, n =>
                sum(n) == n * (n + 1) / 2);
    }
}
