namespace UsingFsCheck2.PrintDiamondKata;

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

    private static IList<string> Implementation1(char upTo)
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

    internal static IList<string> Diamond(char upTo) =>
        Implementation1(upTo);
}
