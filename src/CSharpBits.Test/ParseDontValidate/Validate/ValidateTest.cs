using System;
using System.Collections.Generic;

namespace CSharpBits.Test.ParseDontValidate.Validate;

public class ValidateTest
{
    private readonly DataContext _dataContext = new();
    
    void Create(Movie movie)
    {
        if(string.IsNullOrEmpty(movie.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        if(movie.Rating < 0 || movie.Rating > 5)
        {
            throw new ArgumentException("The rating can't be less than 0 or more than 5.");
        }

        if(movie.Description.Length > 5000)
        {
            throw new ArgumentException("The description is to long.");
        }

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

internal record Movie(
    string? Title,
    int Rating,
    string Description);
