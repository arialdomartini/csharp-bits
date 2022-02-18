using System.Linq;

namespace CSharpBits.Test.Diacritics
{
    internal static class AssertExtensions
    {
        internal static bool OnlyContains(this string s, string allowed) =>
            string.IsNullOrEmpty(s) ||
            s.ToCharArray().All(allowed.Contains);
    }
}