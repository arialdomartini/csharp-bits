using System;
using System.Collections.Generic;
using Xunit;
using static CSharpBits.Test.ParseDontValidate.Parse.MovieParser;

#pragma warning disable CS8509

namespace CSharpBits.Test.ParseDontValidate.Parse;

public class ParseTest
{
    private readonly DataContext _dataContext = new();

    [Fact]
    void invalid_input()
    {
        var inputMovie = new InputMovie(null, 6, new string('x', 10000));

        Error<Movie> result = (Error<Movie>)ParseMovie(inputMovie);

        Assert.Equal("Title is required.", result.ErrorMessage);
    }

    void Create(InputMovie inputMovie)
    {
        Movie movie = Parse(inputMovie);

        Save(movie);
    }

    private Movie Parse(InputMovie inputMovie)
    {
        var parseResult = ParseMovie(inputMovie);

        return parseResult switch
        {
            Error<Movie> error => throw new AggregateException(error.ErrorMessage),
            Success<Movie> success => success.Value
        };
    }

    private void Save(Movie movie)
    {
        _dataContext.Movies.Add(movie);
        _dataContext.SaveChanges();
    }
}

internal class DataContext
{
    internal List<Movie> Movies { get; set; }

    internal void SaveChanges()
    {
    }
}
