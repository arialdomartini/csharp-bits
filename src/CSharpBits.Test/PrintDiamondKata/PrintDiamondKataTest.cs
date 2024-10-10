using System;
using System.Linq;

namespace CSharpBits.Test.PrintDiamondKata;

using FsCheck;
using FsCheck.Xunit;
using Xunit;

//             1111
//    1234567890123 
// 1 "      a      "
// 2 "     b b     "
// 3 "    c   c    "
// 4 "   d     d   "
// 5 "  e       e  "
// 6 " f         f "
// 7 "g           g"
// 8 " f         f "
// 9 "  e       e  "
//10 "   d     d   "
//11 "    c   c    "
//12 "     b b     "
//13 "      a      "

//
//   1234567
//1 "      a"
//2 "     b "
//3 "    c  "
//4 "   d   "
//5 "  e    "
//6 " f     " 
//7 "g      "

// 1. Un quadrato
// 2. contenente principalmente spazi
// 3. di lato 2*target-1
// 1. Semi-simmetrica orizzontalmente
// 2. Semi-simmetrica verticalmente

// Quarto
// 3. un quadrato
// 4. contiene tutte le lettere up-to target
// 5. size = numero di lettere
// 6. ogni riga contiene un trailing space in più
// 7. ogni riga contiene un leading space in meno

class PrintDiamond
{
    internal static string Print(char target)
    {
        return " ";
    }
}

internal static class TestHelper
{
    internal static string[] Lines(this string s) =>
        s.Split("\n").ToArray();
}

public class PrintDiamondKataTest
{
    delegate bool MyProp(char target, string[] strings);
    
    private static Gen<char> Chars =>
        from c in Arb.Generate<char>()
        where c >= 'b'
        where c <= 'z'
        select c;

    private static Arbitrary<char> TargetChars => Chars.ToArbitrary();

    private static Property Verify(MyProp property) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = PrintDiamond.Print(target);

            var lines = diamond.Lines();

            return property(target, lines);
        });

    [Property]
    Property is_a_square() => 
        Verify((_, lines) => 
            lines.ForAll(line => line.Length == lines.Length));
}
