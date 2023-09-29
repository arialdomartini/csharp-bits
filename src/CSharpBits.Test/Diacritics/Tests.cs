using System.Linq;
using FluentAssertions;
using FsCheck.Xunit;
using Xunit;

namespace CSharpBits.Test.Diacritics;

public class Tests
{
    [Theory]
    [InlineData("_", "-")]
    [InlineData("ä", "ae")]
    [InlineData("ü", "ue")]
    [InlineData("ö", "oe")]
    [InlineData("ß", "ss")]
    [InlineData("Ü", "Ue")]
    [InlineData("Ö", "Oe")]
    [InlineData("Ä", "Ae")]
    [InlineData("ç", "c")]
    [InlineData("Ç", "C")]
    [InlineData("ê", "e")]
    [InlineData("Ê", "E")]
    [InlineData("ï", "i")]
    [InlineData("Ï", "I")]
    [InlineData("Ë", "E")]
    [InlineData("ë", "e")]
    [InlineData("ñ", "n")]
    [InlineData("Ñ", "N")]
    [InlineData("Õ", "O")]
    [InlineData("õ", "o")]
    [InlineData("à", "a")]
    [InlineData("è", "e")]
    [InlineData("é", "e")]
    [InlineData("ì", "i")]
    [InlineData("ò", "o")]
    [InlineData("ù", "u")]
    [InlineData("À", "A")]
    [InlineData("È", "E")]
    [InlineData("É", "E")]
    [InlineData("Ì", "I")]
    [InlineData("Ò", "O")]
    [InlineData("Ù", "U")]
    [InlineData("", "")]
    void replacement_of_single_letters(string original, string expected)
    {
        var result = original.Converted();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Üx", "Uex")]
    [InlineData("Öx", "Oex")]
    [InlineData("Äx", "Aex")]
    void german_characters_followed_by_lowercase(string original, string expected)
    {
        var result = original.Converted();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Ü1", "Ue1")]
    [InlineData("Ö2", "Oe2")]
    [InlineData("Ä3", "Ae3")]
    void german_characters_followed_by_numbers(string original, string expected)
    {
        var result = original.Converted();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("ÜX", "UEX")]
    [InlineData("ÖX", "OEX")]
    [InlineData("ÄX", "AEX")]
    void german_characters_followed_by_capital_letter(string original, string expected)
    {
        var result = original.Converted();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("ÜX ÖX ÄX Üx Öx Äx", "UEX OEX AEX Uex Oex Aex")]
    void with_converted_special_cases(string original, string expected)
    {
        var result = original.WithConvertedSpecialCases();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("a string only containing ASCII values 123", "a string only containing ASCII values 123")]
    void should_be_kept(string original, string expected)
    {
        var result = original.Converted();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("ÄPFEL", "AEPFEL")] // Ä followed by capital letter
    [InlineData("Äpfel", "Aepfel")] // Ä followed by lowercase letter
    [InlineData("       ", "       ")]
    void some_examples(string original, string expected)
    {
        var result = original.Converted();

        result.Should().Be(expected);
    }


    [Theory]
    [InlineData("|")]
    [InlineData("+")]
    [InlineData("!")]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("$")]
    [InlineData("%")]
    [InlineData("^")]
    [InlineData("&")]
    [InlineData("*")]
    [InlineData("")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData(",")]
    [InlineData(".")]
    [InlineData("<")]
    [InlineData(">")]
    [InlineData("?")]
    [InlineData("\"")]
    void some_characters_to_be_removed(string original)
    {
        var result = original.Converted();

        result.Should().Be(string.Empty);
    }

    [Property]
    public bool only_contains_allowed_characters(string original)
    {
        var normalized = original.Converted();

        // a-zA-Z 0-9 - ' space
        return normalized
            .OnlyContains("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-' ");
    }

    [Theory]
    [InlineData("(")]
    public void does_not_contain_forbidden_characters(string original)
    {
        var normalized = original.Converted();

        var result = normalized
            .OnlyContains("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-' ");

        normalized.Should().Be("");
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData('a', true)]
    [InlineData(' ', true)]
    [InlineData('X', true)]
    [InlineData('1', true)]
    [InlineData('[', false)]
    [InlineData('Ü', false)]
    [InlineData('Ä', false)]
    void is_allowed(char c, bool expectd)
    {
        DiacriticsConverter.IsAllowed(c).Should().Be(expectd);
    }

    [Theory]
    [InlineData("abcdf", "abcdf")]
    [InlineData("Ü hello!??Ä", " hello")]
    [InlineData("{-[-(-ciao-)-]-}", "---ciao---")]
    [InlineData("\u0230ciao", "ciao")]
    [InlineData("\u0011xyz", "xyz")]
    void remove_not_allowed(string s, string expected)
    {
        var removeNotAllowed = s.RemoveNotAllowed();

        new string(removeNotAllowed.ToArray()).Should().Be(expected);
    }


    [Theory]
    [InlineData("Ü hello!?? Ä", "Ue hello Ae")]
    [InlineData("{-[-(-ciao-)-]-}", "---ciao---")]
    [InlineData("\u0230ciao", "ciao")]
    [InlineData("\u0011xyz", "xyz")]
    void when_converting_it_removes_not_allowed(string s, string expected)
    {
        var removeNotAllowed = s.Converted();

        new string(removeNotAllowed.ToArray()).Should().Be(expected);
    }
}