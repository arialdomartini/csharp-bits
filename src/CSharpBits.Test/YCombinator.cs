using System;
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
        
        // This is replaced by Y(f)
        // private static readonly Sum ad_hoc_continuation = n => n * (n + 1) / 2;


        // A function that given a PartSum returns a Sum.
        // By definition, Y(f) is a correct sum. So we can replace ad_hoc_implementation with it, and
        // feed it to PartSum
        
        // This does not work in C#, causing a Stack Overflow, because C# is strict, not lazy
        private static readonly Func<PartSum, Sum> Y = 
            f => 
                f(Y(f));

        // private static readonly Sum sum = mkSum(ad_hoc_continuation);
        private static readonly Sum sum = Y(mkSum);
        
        [Property]
        Property it_meets_the_gauss_formula() =>
            ForAll(PositiveNumbers, n =>
                sum(n) == n * (n + 1) / 2);
    }
}
