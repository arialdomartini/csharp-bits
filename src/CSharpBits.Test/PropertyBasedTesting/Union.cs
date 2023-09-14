using System;
using Xunit;

namespace CSharpBits.Test.PropertyBasedTesting;

public abstract class GetUserResult
{
}

public record User(string Name);

public sealed class UserFoundResult : GetUserResult
{
    public User User { get; }

    public UserFoundResult(User user)
    {
        User = user;
    }
}

public sealed class UserMissingResult : GetUserResult
{
    public string MissingId { get; }

    public UserMissingResult(string id)
    {
        MissingId = id;
    }
}

public sealed class ErrorResult : GetUserResult
{
    public Exception Cause { get; }

    public ErrorResult(Exception cause)
    {
        Cause = cause;
    }
}

public class Union
{
    [Fact]
    void can_patter_match()
    {
        GetUserResult x = new ErrorResult(new Exception());

        var r = x switch
        {
            ErrorResult errorResult => "error",
            UserFoundResult userFoundResult => "user found",
            UserMissingResult userMissingResult => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(x))
        };
    }
}
