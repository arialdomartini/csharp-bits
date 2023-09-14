using System;
using Xunit;
using static CSharpBits.Test.PropertyBasedTesting.WithRecords.GetUserResult;

namespace CSharpBits.Test.PropertyBasedTesting.WithRecords;

public record User(string Name);

public abstract record GetUserResult
{
    internal sealed record UserFound(User User) : GetUserResult;

    internal sealed record UserMissing(string Id) : GetUserResult;

    internal sealed record Error(Exception Cause) : GetUserResult;
}

public class Union
{
    [Fact]
    void can_patter_match()
    {
        GetUserResult x = new UserMissing(Id: "mario");

        var r = x switch
        {
            Error { Cause: var e } => $"{e}",
            UserFound { User: var u } userFound => $"found {u}",
            UserMissing { Id: var id } userMissing => $"Can't find {id}"
        };
        
        
        Assert.Equal("Can't find mario", r);
    }
}
