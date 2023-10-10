using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpBits.Test.ReaderMonad.ToReaderMonad.Step1;
using Xunit;

namespace CSharpBits.Test.MonadsForTheRestOfsUs;

class IOMonad<T>
{
    private readonly T _value;

    internal IOMonad(T value)
    {
        _value = value;
    }

    internal T Run()
    {
        PlayingWithMonads.Logs.Add($"Invoked with value {_value}");
        return _value;
    }
}

record IO<B>(B value, Action action)
{
    internal B Run()
    {
        action.Invoke();
        return value;
    }
}

class ReadMonad<Extra, Output>
{
    private readonly Func<Extra, Output> _func;

    public ReadMonad(Func<Extra, Output> func)
    {
        _func = func;
    }

    public Output Run(Extra extraParameter)
    {
        Output output = _func(extraParameter);
        return output;
    }
}

public class PlayingWithMonads
{
    internal static readonly IList<string> Logs = new List<string>();

    public PlayingWithMonads()
    {
        File.Delete("output.txt");
    }
    
    int MyLength(string s)
    {
        return s.Length;
    }

    [Fact]
    void simple_function_application()
    {
        Func<string, int> f = s => s.Length;

        string a = "foo";

        Func<Func<string, int>, string, int> apply = (f, a) => f(a);

        int Apply(Func<string, int> f, string a) => f(a);

        var length = apply(f, a);

        Assert.Equal(3, length);
    }

    [Fact]
    void simple_generic_function_application()
    {
        Func<string, int> f = s => s.Length;

        string a = "foo";

        B apply<A, B>(Func<A, B> f, A a) => f(a);
        
        int length = apply(f, a);
        Assert.Equal(3, length);
    }
    
    [Fact]
    void apply_for_linking_functions()
    {
        B Apply<A, B>(Func<A, B> f, A a) => f(a);

        int Length(string s) => s.Length;
        double Double(int i) => i * 2;

        string a = "foo";

        var doubleTheLength = Apply(Double, Apply(Length, "foo"));
        
        Assert.Equal(6, doubleTheLength);
    }

    [Fact]
    void compose_in_terms_of_apply()
    {
        B Apply<A, B>(Func<A, B> f, A a) => f(a);
        
        Func<A, C> Compose<A, B, C>(Func<B, C> g, Func<A, B> f) => a => Apply(g, Apply(f, a));
        
        int Length(string s) => s.Length;
        double Double(int i) => i * 2;

        Func<string, double> composed = Compose<string, int, double>(Double, Length);
        
        var doubleTheLength = composed("foo");
        
        Assert.Equal(6, doubleTheLength);
    }
    
    [Fact]
    void simple_function_composition()
    {
        {
            Func<string, int> length = s => s.Length;
            Func<int, decimal> halfOf = n => (decimal)n / 2;

            var halfTheLength = halfOf(length("foo"));

            Assert.Equal(1.5M, halfTheLength);
        }
        {
            Func<string, decimal> halfOfLength = s =>
            {
                var l = s.Length;
                var halfOfIt = (decimal)l / 2;
                return halfOfIt;
            };

            var halfTheLength = halfOfLength("foo");

            Assert.Equal(1.5M, halfTheLength);
        }
    }

    [Fact]
    void simple_function_composition_with_HOF()
    {
        Func<string, int> length = s => s.Length;
        Func<int, decimal> halfOf = n => (decimal)n / 2;

        Func<string, decimal> compose(Func<int, decimal> halfOf, Func<string, int> length) => s => halfOf(length(s));
        
        var halfOfLength = compose(halfOf, length);
        
        var halfTheLength = halfOfLength("foo");

        Assert.Equal(1.5M, halfTheLength);
    }
    
    [Fact]
    void general_simple_function_composition_with_HOF()
    {
        Func<string, int> length = s => s.Length;
        Func<int, decimal> halfOf = n => (decimal)n / 2;

        Func<A, C> compose<A, B, C>(Func<B, C> g, Func<A, B> f) => s => g(f(s));
        
        var halfOfLength = compose(halfOf, length);
        
        var halfTheLength = halfOfLength("foo");

        Assert.Equal(1.5M, halfTheLength);
    }
    
    [Fact]
    void dishonest_function()
    {
        int b = 0;
        Func<int, int> closure = a => a + b;

        Assert.Equal(1, closure(1));

        b = 42;

        Assert.Equal(43, closure(1));
    }
    
    [Fact]
    void dishonest_function_with_string()
    {
        var b = string.Empty;
        int Closure(string a) => a.Length + b.Length;

        Assert.Equal(3, Closure("foo"));

        b = "wat?";

        Assert.Equal(7, Closure("foo"));
    }
    
    [Fact]
    void dishonest_division()
    {
        decimal Divide(decimal n, decimal d) => n / d;

        Assert.Equal(3M, Divide(9M, 3M));
        Assert.Equal(4.5M, Divide(9M, 2M));

        Assert.Throws<DivideByZeroException>(() => Divide(9M, 0M));
    }
    
    [Fact]
    void run_monadic_function()
    {
        IO<int> CalculateWithSideEffect(string s) =>
            new(
                s.Length,
                () => File.WriteAllText("output.txt", "I'm a side effect!"));
        
        IO<int> monadicValue = CalculateWithSideEffect("foo");
        
        Assert.False(File.Exists("output.txt"));
        
        var result = monadicValue.Run();

        Assert.Equal(3, result);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void bind_monadic_functions()
    {
        IOMonad<int> MonadicLength(string s) =>
            new IOMonad<int>(s.Length);

        IOMonad<int> MonadicDouble(int n) =>
            new IOMonad<int>(n * 2);


        IOMonad<int> monadicLength = MonadicLength("hello");

        Func<string, IOMonad<int>> composition =
            compose<string, int, int>(MonadicLength, MonadicDouble);

        var result = composition("hello").Run();

        Assert.Equal(10, result);
        Assert.Contains("Invoked with value 5", Logs);
        Assert.Contains("Invoked with value 10", Logs);
    }

    private Func<a, IOMonad<c>> compose<a, b, c>(
        Func<a, IOMonad<b>> monadicLength,
        Func<b, IOMonad<c>> monadicDouble) =>
        s =>
        {
            var ioMonad = monadicLength(s);
            var length = ioMonad.Run();
            return monadicDouble(length);
        };


    [Fact]
    void reader_monad()
    {
        Func<string, int> myLength = s => s.Length;
        Func<string, ReadMonad<Guid, int>> myMonadicLength = s =>
            new ReadMonad<Guid, int>(guid =>
                myLength(s)
                + guid.ToString().Length);

        ReadMonad<Guid, int> monadicLength = myMonadicLength("foo");

        var dependency = Guid.NewGuid();
        var result = monadicLength.Run(dependency);

        Assert.Equal(3 + 36, result);
    }

    [Fact]
    void composing_reader_monads()
    {
        Func<string, ReadMonad<Guid, int>> myMonadicLength = s =>
            new ReadMonad<Guid, int>(guid => s.Length + guid.ToString().Length);

        Func<int, ReadMonad<Guid, int>> myMonadicDouble = i =>
            new ReadMonad<Guid, int>(guid => i * 2 + guid.ToString().Length * i);

        // f . g  => f(g(x))

        Func<string, ReadMonad<Guid, int>> combine(Func<int, ReadMonad<Guid, int>> @double, Func<string, ReadMonad<Guid, int>> len)
        {
            return s => new ReadMonad<Guid, int>(extra =>
            {
                ReadMonad<Guid, int> readMonad = len(s);
                int l = readMonad.Run(extra);
                ReadMonad<Guid, int> monad = @double(l);
                int run = monad.Run(extra);
                return run;
            });
        }

        // string -> ReadMonad<Guid, int>
        Func<string, ReadMonad<Guid, int>> combined = combine(myMonadicDouble, myMonadicLength);

        var dependency = Guid.NewGuid();
        ReadMonad<Guid, int> monadicValue = combined("foo");
        var result = monadicValue.Run(dependency);

        Assert.Equal(39 * 2 + (36 * 39), result);
    }


    [Fact]
    void list_monad()
    {
        int myPureLength(string s) => s.Length;

        NonDeterministic myNoDeterministicLength(string s) => new(s.Length, s.Length + 1, 42);

        NonDeterministic noDeterministicLength = myNoDeterministicLength("foo");

        var values = noDeterministicLength.Run();

        Assert.Equal(new[] { 3, 4, 42 }, values);
    }

    [Fact]
    void combine_list_monad()
    {
        NonDeterministic notDeterministicLength(string s) => new(s.Length, s.Length + 1, 42);
        NonDeterministic notDeterministicMult(int i) => new(i * 2, i * 3);

        // Func<string, NonDeterministic>
        Func<string, NonDeterministic> bind(Func<int, NonDeterministic> after, Func<string, NonDeterministic> before)
        {
            return s =>
            {
                IEnumerable<int> enumerable = before(s).Run();
                IEnumerable<int> allResults = enumerable.Select(after).SelectMany(i => i.Run());
                // IEnumerable<int> nonDeterministics = enumerable.SelectMany( i => after(i).Run());
                return new NonDeterministic(allResults.ToArray());
            };
        }

        var combined = bind(notDeterministicMult, notDeterministicLength);

        NonDeterministic noDeterministicLength = combined("foo");

        var values = noDeterministicLength.Run();

        Assert.Equal(new[] { 6, 9, 8, 12, 84, 126 }, values);
    }

    [Fact]
    void apply_nonDetermisticValue_to_nonDeterministicFunction()
    {
        NonDeterministic notDeterministicLength(string s) => new(s.Length, s.Length + 1, 42);

        NonDeterministic nonDetLength = notDeterministicLength("foo");

        NonDeterministic notDeterministicMult(int i) => new(i * 2, i * 3);

        NonDeterministic result = apply(nonDetLength, notDeterministicMult);

        NonDeterministic apply(NonDeterministic nv, Func<int, NonDeterministic> f)
        {
            IEnumerable<int> values = nv.Run();
            IEnumerable<NonDeterministic> nonDeterministics = values.Select(f);

            IEnumerable<int> selectMany = nonDeterministics.SelectMany(i => i.Run());
            return new NonDeterministic(selectMany.ToArray());
        }

        var values = result.Run();

        Assert.Equal(new[] { 6, 9, 8, 12, 84, 126 }, values);
    }

    [Fact]
    void without_container_apply_nonDetermisticValue_to_nonDeterministicFunction()
    {
        List<int> notDeterministicLength(string s) => new() { s.Length, s.Length + 1, 42 };

        List<int> nonDetLength = notDeterministicLength("foo");

        List<int> notDeterministicMult(int i) => new() { i * 2, i * 3 };

        List<int> result = apply(nonDetLength, notDeterministicMult);

        List<int> apply(List<int> nv, Func<int, List<int>> f)
        {
            IEnumerable<int> values = nv;
            IEnumerable<List<int>> nonDeterministics = values.Select(f);

            var many = nonDeterministics.SelectMany(i => i);
            return many.ToList();
        }

        IEnumerable<int> applySimplified(List<int> nv, Func<int, List<int>> f) =>
            nv.SelectMany(f);

        var values = result;

        Assert.Equal(new[] { 6, 9, 8, 12, 84, 126 }, values);
    }
}

class NonDeterministic
{
    private readonly int[] _lengths;

    public NonDeterministic(params int[] lengths)
    {
        _lengths = lengths;
    }

    public IEnumerable<int> Run()
    {
        return _lengths;
    }
}
