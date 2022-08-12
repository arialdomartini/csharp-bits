using System;
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

        private delegate Sum SumC(Sum sum);

        private static readonly SumC mySum =
            f =>
                i =>
                    i == 0 ? 0 : i + f(i - 1);

        private static readonly Func<SumC, Sum> Y =
            f =>
                n =>
                    new Func<MkSum, Sum>(p => p(p))(
                        self =>
                            f(i => 
                                self(self)(i)))(n);

        private static readonly Sum sum = Y(mySum);


        [Property]
        Property it_meets_the_gauss_formula() =>
            ForAll(PositiveNumbers, n =>
                sum(n) == n * (n + 1) / 2);
    }
}
