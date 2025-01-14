using System;
using static CSharpBits.Test.ParseDontValidate.Parse.Result<CSharpBits.Test.ParseDontValidate.Parse.Description>;
using static CSharpBits.Test.ParseDontValidate.Parse.ResultExtensions;

namespace CSharpBits.Test.ParseDontValidate.Parse;

internal record Movie(
    Title Title,
    Rating Rating,
    Description Description)
{
}

internal static class MovieParser
{
    internal static readonly Func<Title, Rating, Description, Movie> Make =
        (title, rating, description) =>
            new(title, rating, description);

    internal static Result<Movie> ParseMovie(InputMovie inputMovie)
    {
        var title = Title.Parse(inputMovie.Title);
        var rating = Rating.Parse(inputMovie.Rating);
        var description = Description.Parse(inputMovie.Description);

        return
            pure(curry(Make))
                .apply(title)
                .apply(rating)
                .apply(description);
    }

}

internal abstract record Rating
{
    internal record Rating1 : Rating;
    internal record Rating2 : Rating;
    internal record Rating3 : Rating;
    internal record Rating4 : Rating;
    internal record Rating5 : Rating;

    static internal Result<Rating> Parse(int value) =>
        value switch
        {
            1 => Result<Rating>.Success(new Rating1()),
            2 => Result<Rating>.Success(new Rating2()),
            3 => Result<Rating>.Success(new Rating3()),
            4 => Result<Rating>.Success(new Rating4()),
            5 => Result<Rating>.Success(new Rating5()),
            _ => Result<Rating>.Error("The rating can't be less than 0 or more than 5.")
        };
}

internal record Title(
    string Value)
{
    internal static Result<Title> Parse(string value)
    {
        if (value == null)
            return Result<Title>.Error("Title is required.");

        return Result<Title>.Success(new Title(value));
    }
}

internal record Description
{
    internal string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    internal static Result<Description> Parse(string value)
    {
        if (value.Length > 5000)
            return Error("The description is to long.");

        return Success(new Description(value));
    }
}
