using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using static FsCheck.Prop;

namespace CSharpBits.Test
{
    public class YCombinator
    {
        private readonly Arbitrary<int> PositiveNumbers = Arb.From(Gen.Choose(0, 15_000));

        private delegate int Sum(int n);
        private delegate Sum PartSum(Sum continuation);

        private static readonly PartSum mkSum =
            continuation =>
                n =>
                    n == 0 ? 0 : n + continuation(n - 1);
        
        private static readonly Sum ad_hoc_continuation = n => n * (n + 1) / 2;
        
        // Note: order does matter! To make it order-independent, use
        // private static readonly Sum sum = n => mkSum(ad_hoc_continuation)(n);
        private static readonly Sum sum = mkSum(ad_hoc_continuation);
        
        [Property]
        Property it_meets_the_gauss_formula() =>
            ForAll(PositiveNumbers, n =>
                sum(n) == n * (n + 1) / 2);
    }
}
