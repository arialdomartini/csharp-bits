using FsCheck;
using FsCheck.Xunit;
using static UsingFsCheck2.PrintDiamondKata.DiamondExtensions;

namespace UsingFsCheck2.PrintDiamondKata;

delegate bool MyProp(IList<string> rows, char upTo);

static class TestExtensions
{
    internal static string Joined(this IEnumerable<string> rows) =>
        string.Join("", rows);

    internal static string Stringified(this IEnumerable<char> cs) =>
        string.Join("", cs);

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

    internal static int InnerSpaces(this string e)
    {
        var withoutLeadingSpaces = e
            .SkipWhile(c => c == space)
            .Stringified();
        var withoutTrailingSpaces =
            withoutLeadingSpaces
                .Reverse()
                .SkipWhile(c => c == space)
                .Stringified();
        var withoutNonSpaces = withoutTrailingSpaces
            .Where(c => c == space)
            .Stringified();
        
        return withoutNonSpaces.Length;
    }

    private static string WithoutSpaces(this string s) =>
        string.Join("", s.Where(c => c != space));

    internal static IList<string> IgnoringSpaces(this IEnumerable<string> s) =>
        s.Select(WithoutSpaces).ToList();

    internal static bool IsOdd(this int i) => i % 2 == 1;
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
    Property contains_only_chars_up_to_target() =>
        CheckProperty((list, upToChar) =>
            list.DifferentLetters().ForAll(c => c >= 'a' && c <= upToChar));

    [Property]
    Property contains_all_of_them() =>
        CheckProperty((list, upToChar) =>
            list.DifferentLetters().Count() == upToChar - 'a' + 1);

    [Property]
    Property distributed_in_2_times_minus_1_lines()
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
    Property the_whole_list_is_symmetric() =>
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
    Property each_line_is_a_palyndrom() =>
        CheckProperty((list, _) =>
            list.ForAll(line => line.SequenceEqual(line.Reverse())));

    [Property]
    Property center_element_contains_no_surrounding_spaces() =>
        CheckProperty((list, _) =>
        {
            var center = list.Center();
            return center.Trim() == center;
        });

    [Property]
    Property peripheral_elements_contains_no_inner_spaces() =>
        CheckProperty((list, _) =>
        {
            var first = list.First();
            var last = list.Last();
            var innerSpaces = first.InnerSpaces();
            return innerSpaces == 0 && last.InnerSpaces() == 0;
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
    Property each_line_contains_2_inner_spaces_less_than_the_next_one() =>
        CheckProperty((list, _) =>
        {
            var firstHalf = list.FirstHalf().Skip(1);
            var shifted = list.FirstHalf().Skip(2);
            var together = firstHalf.Zip(shifted);

            return together.ForAll(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                var innerSpaces = previous.InnerSpaces();
                var nextInnerSpaces = next.InnerSpaces();
                return innerSpaces == nextInnerSpaces - 2;
            });
        });

    [Property]
    Property all_lines_have_the_same_length() =>
        CheckProperty((list, _) =>
        {
            var x = list.Select(l => (l, l.Length));
            return list.Select(el => el.Length).Distinct().Count() == 1;
        });

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
        CheckProperty((list, _) => { return list.ForAll(el => el.Length().IsOdd()); });

    #endregion
}
