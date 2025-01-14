namespace CSharpBits.Test.ParseDontValidate.Parse;

internal abstract record Option<T>;

internal record Just<T>(
    T Value) : Option<T>;

internal record None<T>() : Option<T>;
