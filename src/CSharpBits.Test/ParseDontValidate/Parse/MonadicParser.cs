namespace CSharpBits.Test.ParseDontValidate.Parse;

#pragma warning disable CS8509
internal static class MonadicParser
{
    internal static Result<Movie> ParseMovieMonadic(InputMovie inputMovie) =>
        from title in Title.Parse(inputMovie.Title)
        from rating in Rating.Parse(inputMovie.Rating)
        from description in Description.Parse(inputMovie.Description)
        select new Movie(title, rating, description);
}
