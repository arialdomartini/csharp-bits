namespace CSharpBits.Test.ParseDontValidate.Parse;

#pragma warning disable CS8509
internal abstract record Result<T>
{
    internal static Result<T> Success(T value) => new Success<T>(value);
    internal static Result<T> Error(string errorMessage) => new Error<T>(errorMessage);
}

internal record Success<T>(
    T Value) : Result<T>;

internal record Error<T>(
    string ErrorMessage) : Result<T>;
