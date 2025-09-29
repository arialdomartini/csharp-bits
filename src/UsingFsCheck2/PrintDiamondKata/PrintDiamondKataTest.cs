using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;
using static CSharpBits.Test.PrintDiamondKata.PrintDiamond;

namespace CSharpBits.Test.PrintDiamondKata;

using FsCheck;
using FsCheck.Xunit;

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

// - Un quadrato
// - contenente principalmente spazi
// - di lato 2*target-1
// - Semi-simmetrica orizzontalmente
// - Semi-simmetrica verticalmente

// Quarto
// - un quadrato
// - contiene tutte le lettere up-to target
// - size = numero di lettere
// - ogni riga contiene un trailing space in più
// - ogni riga contiene un leading space in meno

static class PrintDiamond
{
    internal const char Space = '-';
    internal const char Newline = '\n';

    internal static string Print(char target)
    {
        var n = target - 'a' + 1;

        return
            Range(0, n)
                .Select(index =>
                    BuildLine(index, n))
                .Select(SemiDuplicate)
                .SemiDuplicate()
                .Joined();
    }

    private static IEnumerable<char> BuildLine(int index, int n) => 
        Spaces(n-index-1).Append((char)('a' + index)).Append(Spaces(index));

    private static IEnumerable<char> Spaces(int numberOfSpaces) =>
        Repeat(Space, numberOfSpaces);

    private static string Joined(this IEnumerable<IEnumerable<char>> lines) => 
        string.Join(Newline, lines);

    private static IEnumerable<T> SemiDuplicate<T>(this IEnumerable<T> xs) => 
        xs.Append(xs.Reverse().Skip(1));
}

internal static class TestHelper
{
    internal static string[] Lines(this string s) =>
        s.Split(Newline).ToArray();

    internal static char[] ContainedLetters(this string diamond) =>
        diamond
            .Where(c => c != Space)
            .Where(c => c != Newline)
            .Distinct()
            .Order()
            .ToArray();

    internal static string Reversed(this string line) =>
        new(line.Reverse().ToArray());
    
    
    internal static int LeadingSpaces(this string e) =>
        e.TakeWhile(c => c == Space).Count();

    internal static int TrailingSpaces(this string e) =>
        e.Reverse().TakeWhile(c => c == Space).Count();

}

public class PrintDiamondKataTest
{
    delegate bool MyPropLines(string[] strings);

    delegate bool MyPropFulDiamond(char target, string diamond);

    private static Gen<char> Chars =>
        from c in Arb.Generate<char>()
        where c >= 'b'
        where c <= 'z'
        select c;

    private static Arbitrary<char> TargetChars => Chars.ToArbitrary();

    private static Property ForEachDiamond(MyPropFulDiamond property) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = Print(target);

            return property(target, diamond);
        });

    private static Property ForAllLines(MyPropLines property) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = Print(target);

            var lines = diamond.Lines();

            return property(lines);
        });

    private static Property ForAllLinesInQuarter(MyPropLines property) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = Print(target);

            var lines = diamond.Lines();
            var quarter =
                lines
                    .Take(lines.Length / 2+1)
                    .Select(l => l[..(l.Length / 2 + 1)]).ToArray();

            return property(quarter);
        });

    [Property]
    Property is_a_square() =>
        ForAllLines(lines =>
            lines.ForAll(line => line.Length == lines.Length));

    [Property]
    Property more_spaces_than_letters() =>
        ForEachDiamond((_, diamond) =>
        {
            var spaces = diamond.Count(c => c == Space);
            var letters = diamond.ContainedLetters();
            return spaces >= letters.Length;
        });

    [Property]
    Property contains_all_the_letters_up_to_target() =>
        ForEachDiamond((target, diamond) =>
        {
            var distinctLetters = diamond.ContainedLetters();


            IEnumerable<char> AllLetters(char from, char upTo)
            {
                for (var c = from; c <= upTo; c++)
                    yield return c;
            }

            var expectedLetters = AllLetters('a', target).Order().ToArray();

            return
                distinctLetters.SequenceEqual(expectedLetters);
        });

    [Property]
    Property semi_symmetric_horizontally() =>
        ForAllLines(lines =>
            lines.ForAll(IsPalyndrome));

    private bool IsPalyndrome(string line) =>
        line == line.Reversed();

    [Property]
    Property semi_symmetric_vertically() =>
        ForAllLines(lines =>
            lines.SequenceEqual(lines.Reverse()));

    [Property]
    Property in_each_quarter_each_line_contains_1_leading_space_more_than_the_next_one() =>
        ForAllLinesInQuarter(lines =>
        {
            var firstHalf = lines;
            var shifted = lines.Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.ForAll(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.LeadingSpaces() == next.LeadingSpaces() + 1;
            });
        });
    
    [Property]
    Property in_each_quarter_each_line_contains_1_trailing_space_less_than_the_next_one() =>
        ForAllLinesInQuarter(lines =>
        {
            var firstHalf = lines;
            var shifted = lines.Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.ForAll(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.TrailingSpaces() + 1 == next.TrailingSpaces();
            });
        });
}
