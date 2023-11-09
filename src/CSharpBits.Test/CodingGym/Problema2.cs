using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.CodingGym;

public class Problema2
{
    private delegate IEnumerable<string> ManipulateHistory(IEnumerable<string> history);


    ManipulateHistory append(string tail) =>
        history => history.Append(new[] { history.Last() + tail });

    ManipulateHistory delete(int k) =>
        history => history.Append(new[] { delete(history.Last(), k) });

    string delete(string last, int k) => last.Substring(0, last.Length - k);

    ManipulateHistory print(int k) => history =>
    {
        Console.WriteLine(history.Last()[k - 1].ToString());
        return history;
    };

    ManipulateHistory undo() =>
        history => history.Take(history.Length() - 1);


    ManipulateHistory toCommand(string commandAndParameter)
    {
        var parts = commandAndParameter.Split(" ");
        var command = parts[0];
        
        return command switch
        {
            "1" => append(parts[1]),
            "2" => delete(int.Parse(parts[1])),
            "3" => print(int.Parse(parts[1])),
            "4" => undo(),
        };
    }

    [Fact]
    void pass()
    { 
        var input = new List<string>
        {
            "1 fg", // abcdefg 
            "3 6",  // abcdefg
            "2 5",  // abcdefg    ab   
            "4",    // abcdefg
            "3 7",  // abcdefg
            "4",    // abcde
            "3 4"   // 
        };

        IEnumerable<string> initial = new List<string>{"abcde"};
        
        var result = input.Select(toCommand).Aggregate(initial, (h, c) => c(h));
        
        Assert.Equal("abcde", result.Last());
    }
}
