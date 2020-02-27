using System.Collections.Generic;
using System.Linq;

namespace CSharpBits.Test.FunctionalParser
{
    internal class Result
    {
        internal IEnumerable<string> Tracks { get; private set; } = new List<string>();

        internal Result Track(string name) => new Result {Tracks = Tracks.Union(new[] {name})};
    }
}