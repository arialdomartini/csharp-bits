using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using static CSharpBits.Test.DiamondExtensions;

namespace CSharpBits.Test;

delegate bool MyProp(IList<string> rows, char upTo);

static class TestExtensions
{
    internal static string Joined(this IEnumerable<string> rows) =>
        string.Join("", rows);

    internal static IEnumerable<char> DifferentLetters(this IList<string> rows) =>
        rows.IgnoringSpaces().SelectMany(row => row.Distinct()).Distinct();

    internal static IEnumerable<string> FirstHalf(this IList<string> list) =>
        list.Take(list.Count / 2);

    internal static IEnumerable<string> SecondHalf(this IList<string> list) =>
        list.Skip(list.Count / 2 + 1);

    internal static string Center(this IList<string> list) =>
        list.Skip(list.Count / 2).First();

    internal static IEnumerable<string> Twice(this IEnumerable<string> list) =>
        list.Select(a => $"{a}{a}");

    internal static int LeadingSpaces(this string e) =>
        e.TakeWhile(c => c == space).Count();

    internal static int TrailingSpaces(this string e) =>
        e.Reverse().TakeWhile(c => c == space).Count();

    private static string WithoutSpaces(this string s) =>
        string.Join("", s.Where(c => c != space));

    internal static IList<string> IgnoringSpaces(this IEnumerable<string> s) =>
        s.Select(WithoutSpaces).ToList();

    internal static bool IsOdd(this int i) => i % 2 == 1;
}

static class DiamondExtensions
{
    internal const char space = '*';

    internal static IEnumerable<string> Inside(this IEnumerable<string> list) =>
        list.Skip(1).SkipLast();

    private static IEnumerable<string> Singleton(this string e) =>
        [e];

    private static string Spaces(this int times) =>
        new(space, times);

    private static IList<string> Union(this IEnumerable<string> xs, string s) =>
        xs.Union(s.Singleton()).ToList();

    private static IList<string> Union(this string s, IEnumerable<string> xs) =>
        s.Singleton().ToList().Union(xs).ToList();

    private static (string First, IList<string> Inside, string Last) Split(IList<string> letters) =>
    (
        First: letters.First(),
        Inside: letters.Inside().ToList(),
        Last: letters.Last());

    private static List<string> LettersUpTo(char upTo) =>
        Enumerable.Range('a', upTo - 'a' + 1).Select(c => ((char)c).ToString()).ToList();

    internal static IList<string> Diamond(char upTo)
    {
        var letters = LettersUpTo(upTo);

        var (first, inside, last) = Split(letters);
        var length = letters.Length();

        var firstWithSpaces = $"{(length).Spaces()}{first}{(length).Spaces()}";

        var withSpaces =
            inside
                .Select((s, i) =>
                {
                    var index = i;
                    var innerSpaces = index * 2 + 1;
                    var spaces = ((length * 2 - innerSpaces) / 2);
                    return $"{spaces.Spaces()}{s}{innerSpaces.Spaces()}{s}{spaces.Spaces()}";
                });


        var firstHalf =
            firstWithSpaces
                .Union(withSpaces);

        var innerSpacesLast = length * 2 - 1;
        var lastWithSpaces = $"{last}{innerSpacesLast.Spaces()}{last}";

        var secondHalf = firstHalf.Reverse().ToList();

        return firstHalf
            .Append(lastWithSpaces)
            .Append(secondHalf)
            .ToList();
    }
}

public class DiamondKataTest
{
    private static Gen<char> UpToChars() =>
        from c in Arb.Generate<char>()
        where c >= 'b'
        where c <= 'z'
        select c;

    private Property CheckProperty(MyProp prop)
    {
        var upToChars = UpToChars();
        return Prop.ForAll(upToChars.ToArbitrary(), upToChar =>
        {
            var diamond = Diamond(upToChar);

            return prop(diamond, upToChar);
        });
    }
    
    [Property]
    Property does_not_contain_any_letter_beyond_the_target() =>
        CheckProperty((rows, upTo) =>
            rows.ForAll(s => s.ForAll(c => c <= upTo)));

    [Property]
    Property contains_all_chars_under_the_target_included() =>
        CheckProperty((list, upToChar) =>
            list.DifferentLetters().Count() == upToChar - 'a' + 1);
    
    [Property]
    Property number_of_lines()
    {
        bool NumberOfLinesEqual2TimesDifferentLettersMinus1(IList<string> rows, char _)
        {
            int ExpectedNumberOfLines(int numberOfDifferentLetters) =>
                2 + (numberOfDifferentLetters - 2) * 2 + 1;
            
            return rows.Count == ExpectedNumberOfLines(rows.DifferentLetters().Count());
        }

        return CheckProperty(NumberOfLinesEqual2TimesDifferentLettersMinus1);
    }

    [Property]
    Property no_line_is_empty() => 
        CheckProperty((rows, _) => rows.ForAll(row => !string.IsNullOrEmpty(row)));

    [Property]
    Property in_first_half_all_lines_are_disjointed() =>
        CheckProperty((rows, _) =>
            rows.Aggregate("", (i, s) => string.Join("", i.Intersect(s))).Length == 0);
    
    [Property]
    Property first_half_is_sorted() =>
        CheckProperty((list, _) =>
        {
            IEnumerable<string> firstHalf = list.IgnoringSpaces().FirstHalf().ToList();
            return firstHalf.Order().SequenceEqual(firstHalf);
        });
    
    [Property]
    Property it_is_symmetric() =>
        CheckProperty((list, _) =>
            list.SequenceEqual(list.Reverse()));
    
    [Property]
    Property each_letter_but_extremes_are_repeated_twice() =>
        CheckProperty((list, _) =>
        {
            var inside = list.IgnoringSpaces().FirstHalf().Inside();
            return inside.ForAll(l => l.Length == 2 && l[0] == l[1]);
        });

    [Property]
    Property center_element_contains_no_surrounding_spaces() =>
        CheckProperty((list, _) =>
        {
            var center = list.Center();
            return center.Trim() == center;
        });

    [Property]
    Property each_line_contains_1_leading_space_more_than_the_next_one() =>
        CheckProperty((list, _) =>
        {
            var firstHalf = list.FirstHalf();
            var shifted = list.FirstHalf().Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.ForAll(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.LeadingSpaces() == next.LeadingSpaces() + 1;
            });
        });

    [Property]
    Property each_line_contains_1_trailing_space_more_than_the_next_one() =>
        CheckProperty((list, _) =>
        {
            var firstHalf = list.FirstHalf();
            var shifted = list.FirstHalf().Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.ForAll(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.TrailingSpaces() == next.TrailingSpaces() + 1;
            });
        });

    [Property]
    Property all_lines_have_the_same_length() =>
        CheckProperty((list, _) =>
        {
            var x = list.Select(l => (l, l.Length));
            return list.Select(el => el.Length).Distinct().Count() == 1;
        });

    [Property]
    Property each_line_is_a_palyndrom() =>
        CheckProperty((list, _) =>
            list.ForAll(line => line.SequenceEqual(line.Reverse())));

    #region DoubleCheck

    [Property]
    Property is_surrounded_by_a() =>
        CheckProperty((list, _) =>
        {
            var withoutSpaces = list.IgnoringSpaces();
            var first = withoutSpaces.First();
            var last = withoutSpaces.Last();
            if (!(first == "a" && last == "a"))
                Console.WriteLine();
            return first == "a" && last == "a";
        });

    [Property]
    Property halves_are_symmetric() =>
        CheckProperty((list, _) =>
        {
            var firstHalf = list.FirstHalf();
            var secondHalf = list.SecondHalf();
            return firstHalf.SequenceEqual(secondHalf.Reverse());
        });
    
    [Property]
    Property all_lines_have_an_odd_length() =>
        CheckProperty((list, _) =>
        {
            return list.ForAll(el => el.Length().IsOdd());
        });
    #endregion
}
