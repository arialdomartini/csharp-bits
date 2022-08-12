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
                var f = self(self);
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
