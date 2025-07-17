using System.Linq;
using Xunit;

namespace CSharpBits.Test.HackerRank;

public class StairCase
{
    string Staircase(int n)
    {
        if (n == 1)
        {
            return "#";
        }

        var lists =
            Staircase(n - 1)
            .Split("\r\n")
            .Select(s => $" {s}")
            .ToList();

        return string.Join("\r\n", lists.Append([lists[^1].Replace(" ", "#")]));
    }

    [Fact]
    void case_for_5()
    {
        var expected = @"    #
   ##
  ###
 ####
#####";


        Assert.Equal(expected, Staircase(5));
    }
}
