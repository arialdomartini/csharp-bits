using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpBits.Test.Diacritics
{
    internal static class DiacriticsConverter
    {
        internal static IEnumerable<char> RemoveNotAllowed(this IEnumerable<char> s)
        {
            return s.Where(IsAllowed);
        }

        internal static bool IsAllowed(char c)
        {
            return Regex.IsMatch(c.ToString(), @"[a-zA-Z0-9\s\-]");
        }

        private static string RemoveNotAllowed(this string s)
        {
            return Regex.Replace(s, @"[^a-zA-Z0-9\s\-]", "");
        }

        private static string RemoveControl(this string s)
        {
            return Regex.Replace(s, @"[\u0000-\u001F]", string.Empty);
        }

        internal static string WithConvertedSpecialCases(this string text)
        {
            var maps = new Dictionary<string, string>
            {
                // Lower case
                {"ä",  "ae"},
                {"ö",  "oe"},
                {"ü",  "ue"},

                // Upper case, when followed by lower case
                {"Ä(?=[a-z0-9äöüß ])", "Ae"},
                {"Ö(?=[a-z0-9äöüß ])", "Oe"},
                {"Ü(?=[a-z0-9äöüß ])", "Ue"},

                // Upper case, when followed by upper case
                {"Ä(?=[A-ZÄÖÜ])", "AE"},
                {"Ö(?=[A-ZÄÖÜ])", "OE"},
                {"Ü(?=[A-ZÄÖÜ])", "UE"},

                // Upper case, single letter
                {"Ä",  "Ae"},
                {"Ö",  "Oe"},
                {"Ü",  "Ue"},


                {"ß",  "ss"},

                {"_", "-"},
                {"ç", "c"},
                {"Ç", "C"},
                {"Ë", "E"},
                {"ë", "e"},
                {"ñ", "n"},
                {"Ñ", "N"},
                {"Õ", "O"},
                {"õ", "o"},
                {"à", "a"},
                {"è", "e"},
                {"é", "e"},
                {"Ï", "I"},
                {"ï", "i"},
                {"ì", "i"},
                {"ò", "o"},
                {"ù", "u"},
                {"À", "A"},
                {"Ê", "E"},
                {"ê", "e"},
                {"È", "E"},
                {"É", "E"},
                {"Ì", "I"},
                {"Ò", "O"},
                {"Ù", "U"}
            };

            return
                maps.Aggregate(text, (a, kp) => Regex.Replace(a, kp.Key, kp.Value));
        }

        internal static string Converted(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return
                text
                    .WithConvertedSpecialCases()
                    .RemoveControl()
                    .RemoveNotAllowed();
        }
    }
}