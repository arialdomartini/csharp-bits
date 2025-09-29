using CSharpBits.Test.UsingFsCheck3;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

[assembly: Properties(Arbitrary = [typeof(Generators)])]

namespace CSharpBits.Test.UsingFsCheck3;

[Properties(Arbitrary = [typeof(Generators)])]
public class Basic
{
    [Property]
    bool receiving_argument(int i) => i * i >= 0;

    private Gen<int> Numbers =>
        ArbMap.Default.GeneratorFor<int>();

    private Gen<int> EvenNumbers =>
        from n in Numbers
        select n * 2;

    [Property]
    Property with_generator() =>
        Prop.ForAll(
            EvenNumbers.ToArbitrary(),
            n => n * n >= 0
        );

    // (anyway overwritten by the assembly and the class directives)
    [Property(Arbitrary = [typeof(Generators)])]
    bool with_implicit_generator(int n) =>
        n >= 0;

    [Property]
    bool with_implicit_assembly_level_generator(int n) =>
        n >= 0;
}

class Generators
{
    private static Gen<int> Numbers =>
        ArbMap.Default.GeneratorFor<int>();

    private static readonly Gen<int> PositiveNumbers =
        from n in Numbers
        select n * n;

    static Arbitrary<int> Squares => PositiveNumbers.ToArbitrary();
}
