using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.LeetCode;

public class Triangle
{
    int[][] Sums(int[][] triangle)
    {
        int[][] PathsFrom(int level)
        {
            if (level == triangle.Length - 1)
                return triangle[level].Select(p => (int[])[p]).ToArray();

            var next = PathsFrom(level + 1);

            return
                triangle[level]
                    .Select((thisElement, thisI) => next[thisI].Append(next[thisI+1])
                        .Select(n => n + thisElement).ToArray()).ToArray();
        }

        return PathsFrom(0);
    }



    [Fact]
    void find_all_path_sums()
    {
        int[][] input = [[2],[3,4],[6,5,7],[4,1,8,3]];

        int minumumSum = Sums(input)[0].Min();

        Assert.Equal(11, minumumSum);
    }
}
