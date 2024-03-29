﻿using System;
using FsCheck;
using FsCheck.Xunit;
using static FsCheck.Prop;

namespace CSharpBits.Test;

public class YCombinator
{
    private readonly Arbitrary<int> PositiveNumbers = Arb.From(Gen.Choose(0, 8_000));

    private delegate int Sum(int i);
    private delegate Sum MkSum(Sum f);
    private delegate Sum Rec(Rec rec);
        
    private static readonly Sum sum =
        Y(mkSum);

    private static Sum Y(Func<Sum, Sum> f) =>
        new Func<Rec, Sum>(f => f(f))(self => f(i => self(self)(i)));

    private static Sum mkSum(Sum f) =>
        i =>
            i == 0 ? 0 : i + f(i - 1);

    [Property]
    Property it_meets_the_gauss_formula() =>
        ForAll(PositiveNumbers, n =>
            sum(n) == n * (n + 1) / 2);
}
