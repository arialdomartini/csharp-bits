using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace CSharpBits.Test.PrintDiamondKata;

internal static class QuarterExtensions
{
    internal static IList<string> Quarter(this IList<string> strings) =>
        strings
            .Take(strings.Count / 2 + 1)
            .Select(line => line.Substring(0, line.Length / 2 + 1))
            .ToList();
}

public class DiamondKataStringTest
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
            var diamond = Bits.Diamond.diamond(upToChar).Split("\r\n");

            return prop(diamond, upToChar);
        });
    }

    [Property]
    Property output_is_vertically_symmetric() =>
        CheckProperty((list, _) =>
            list.SequenceEqual(list.Reverse()));
    
    [Property]
    Property output_is_horizontally_symmetric() =>
        CheckProperty((list, _) =>
            list.ForAll(line => line.SequenceEqual(line.Reverse())));
    
    [Property]
    Property quarter_contains_only_chars_up_to_target() =>
        CheckProperty((list, upToChar) =>
            list.Quarter()
                .DifferentLetters()
                .ForAll(c => c >= 'a' && c <= upToChar));

    [Property]
    Property quarter_contains_all_letters_up_to_target() =>
        CheckProperty((list, upToChar) =>
        {
            var quarter = list.Quarter();
            return quarter
                .DifferentLetters().Count() == upToChar - 'a' + 1;
        });

    [Property]
    Property quarter_each_line_exactly_one_letter() =>
        CheckProperty((list, _) =>
            list.Quarter()
                .ForAll(line => line.Distinct().Count() == 1));

    
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


    private int CallNumber = 0;
    
    private int FakeRandom()
    {
        CallNumber++;
        return CallNumber switch
        {
            1 => 2,
            2 => 8,
            3 => 3,
            4 => 4,
            _ => throw new NotImplementedException()
        };
    }

    private Func<int> MakeFakeRandom(List<int> expectedValues)
    {
        var callNumber = 0;

        return () => expectedValues[callNumber++];
    }

    [Fact]
    void fake_random_with_factory()
    {
        // 2 8 3 4 
        var fakeRandom = MakeFakeRandom([2, 8, 3, 4]);
        
        int i1 = fakeRandom();
        Assert.Equal(2, i1);
        
        int i2 = fakeRandom();
        Assert.Equal(8, i2);
        
        int i3 = fakeRandom();
        Assert.Equal(3, i3);
        
        int i4 = fakeRandom();
        Assert.Equal(4, i4);
    }

    [Fact]
    void fake_random()
    {
        // 2 8 3 4 
        int i1 = FakeRandom();
        Assert.Equal(2, i1);
        
        int i2 = FakeRandom();
        Assert.Equal(8, i2);
        
        int i3 = FakeRandom();
        Assert.Equal(3, i3);
        
        int i4 = FakeRandom();
        Assert.Equal(4, i4);
    }
}
