using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CSharpBits.Test.MonadsForTheRestOfsUs;

static class FunctionExtensions
{
    internal static B Apply<A, B>(this Func<A, B> f, A a) => f(a);

    internal static Func<A, C> Compose<A, B, C>(this Func<B, C> g, Func<A, B> f) =>
        a =>
            g.Apply(f.Apply(a));


    internal static Func<A, C> ComposedWith<A, B, C>(this Func<B, C> g, Func<A, B> f) => a => g(f(a));

    internal static IO<B> Apply<A, B>(this Func<A, IO<B>> f, IO<A> a)
        => new(() =>
        {
            IO<B> io = f(a.Run());
            return io.Run();
        });

    internal static Func<A, IO<C>> ComposedWith<A, B, C>(this Func<B, IO<C>> g, Func<A, IO<B>> f) =>
        a => g.Apply(f(a));

    internal static Func<A, IO<C>> ComposedWith2<A, B, C>(this Func<B, IO<C>> g, Func<A, IO<B>> f)
    {
        return a =>
        {
            IO<B> ioB = f(a);
            B b = ioB.Run();
            IO<C> c = g(b);
            return c;
        };
    }

    internal static Nond<T> Return<T>(this IEnumerable<T> items) => new(items);
}

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

public record IO<B>(Func<B> action)
{
    internal B Run()
    {
        return action.Invoke();
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

record Nond<T>(IEnumerable<T> Items)
{
    internal IEnumerable<T> Run() => Items;
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

        B Apply<A, B>(Func<A, B> f, A a) => f(a);

        int length = Apply(f, a);
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
                () =>
                {
                    File.WriteAllText("output.txt", "I'm a side effect!");
                    return s.Length;
                });

        IO<int> monadicValue = CalculateWithSideEffect("foo");

        Assert.False(File.Exists("output.txt"));

        var result = monadicValue.Run();

        Assert.Equal(3, result);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void binding_IO_monadic_functions()
    {
        IO<int> LengthWithSideEffect(string s) =>
            new IO<int>(
                () =>
                {
                    File.WriteAllText("output.txt", "I'm a side effect!");
                    return s.Length;
                });

        IO<double> DoubleWithSideEffect(int n) =>
            new IO<double>(
                () =>
                {
                    File.AppendAllText("output.txt", "I'm another side effect!");
                    return n * 2;
                });


        IO<B> Apply<A, B>(Func<A, IO<B>> f, IO<A> a) => new(() =>
        {
            A aResult = a.Run();
            IO<B> bResult = f(aResult);
            return bResult.Run();
        });

        IO<string> Return(string s) => new(() => s);

        var apply = Apply(LengthWithSideEffect, Return("foo"));

        IO<double> monadicResult = Apply(DoubleWithSideEffect, apply);
        // Indeed, no file has been created yet

        Assert.False(File.Exists("output.txt"));


        var result = monadicResult.Run();

        Assert.Equal(3 * 2, result);

        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void composing_IO_monadic_functions()
    {
        IO<int> LengthWithSideEffect(string s) =>
            new IO<int>(() =>
            {
                File.WriteAllText("output.txt", "I'm a side effect!");
                return s.Length;
            });

        IO<double> DoubleWithSideEffect(int n) =>
            new IO<double>(
                () =>
                {
                    File.AppendAllText("output.txt", "I'm another side effect!");
                    return n * 2;
                });


        IO<B> Apply<A, B>(Func<A, IO<B>> f, IO<A> a) => new(() =>
        {
            A aResult = a.Run();
            IO<B> bResult = f(aResult);
            return bResult.Run();
        });

        IO<A> Return<A>(A s) => new(() => s);

        Func<A, IO<C>> Compose<A, B, C>(Func<B, IO<C>> g, Func<A, IO<B>> f)
        {
            return new Func<A, IO<C>>(a =>
            {
                IO<B> aa = Apply(f, Return(a));
                IO<C> io = Apply(g, aa);
                return io;
            });
        }

        var composed = Compose<string, int, double>(DoubleWithSideEffect, LengthWithSideEffect);

        IO<double> monadicResult = composed("foo");

        var result = monadicResult.Run();

        Assert.Equal(3 * 2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void composing_IO_monadic_functions_with_linq()
    {
        IO<int> LengthWithSideEffect(string s) =>
            new IO<int>(
                () =>
                {
                    File.WriteAllText("output.txt", "I'm a side effect!");
                    return s.Length;
                });

        IO<double> DoubleWithSideEffect(int n) =>
            new IO<double>(
                () =>
                {
                    File.AppendAllText("output.txt", "I'm another side effect!");
                    return n * 2;
                });


        IO<double> monadicResult =
            from l in LengthWithSideEffect("foo")
            from d in DoubleWithSideEffect(l)
            select d;

        var result = monadicResult.Run();

        Assert.Equal(3 * 2, result);
        Assert.Equal("I'm a side effect!I'm another side effect!", File.ReadAllText("output.txt"));
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

        Nond<int> myNoDeterministicLength(string s) => new(new[] { s.Length, s.Length + 1, 42 });

        Nond<int> noDeterministicLength = myNoDeterministicLength("foo");

        var values = noDeterministicLength.Run();

        Assert.Equal(new[] { 3, 4, 42 }, values);
    }

    [Fact]
    void combine_list_monad()
    {
        Nond<int> notDeterministicLength(string s) => new(new[] { s.Length, s.Length + 1, 42 });
        Nond<int> notDeterministicMult(int i) => new (new[] { i * 2, i * 3 });

        // Func<string, NonDeterministic<int>>
        Func<string, Nond<int>> bind(Func<int, Nond<int>> after, Func<string, Nond<int>> before)
        {
            return s =>
            {
                IEnumerable<int> enumerable = before(s).Run();
                IEnumerable<int> allResults = enumerable.Select(after).SelectMany(i => i.Run());
                // IEnumerable<int> NonDeterministic<int>s = enumerable.SelectMany( i => after(i).Run());
                return new Nond<int>(allResults.ToArray());
            };
        }

        var combined = bind(notDeterministicMult, notDeterministicLength);

        Nond<int> noDeterministicLength = combined("foo");

        var values = noDeterministicLength.Run();

        Assert.Equal(new[] { 6, 9, 8, 12, 84, 126 }, values);
    }

    [Fact]
    void apply_nonDetermisticValue_to_nonDeterministicFunction()
    {
        Nond<int> notDeterministicLength(string s) => new (new[] { s.Length, s.Length + 1, 42});

        Nond<int> nonDetLength = notDeterministicLength("foo");

        Nond<int> notDeterministicMult(int i) => new (new []{ i * 2, i * 3 });

        Nond<int> result = apply(nonDetLength, notDeterministicMult);

        Nond<int> apply(Nond<int> nv, Func<int, Nond<int>> f)
        {
            IEnumerable<int> values = nv.Run();
            IEnumerable<Nond<int>> nonDeterministics = values.Select(f);

            IEnumerable<int> selectMany = nonDeterministics.SelectMany(i => i.Run());
            return new Nond<int>(selectMany.ToArray());
        }

        var values = result.Run();

        Assert.Equal(new[] { 6, 9, 8, 12, 84, 126 }, values);
    }

    [Fact]
    void knight_movement()
    {
        Nond<int> notDeterministicLength(string s) => new(new[] { s.Length, s.Length + 1, 42 });

        Nond<int> nonDetLength = notDeterministicLength("foo");

        Nond<int> notDeterministicMult(int i) => new(new[] { i * 2, i * 3 });

        Nond<int> result = apply(nonDetLength, notDeterministicMult);

        Nond<int> apply(Nond<int> nv, Func<int, Nond<int>> f)
        {
            IEnumerable<int> values = nv.Run();
            IEnumerable<Nond<int>> nonDeterministics = values.Select(f);

            IEnumerable<int> selectMany = nonDeterministics.SelectMany(i => i.Run());
            return new Nond<int>(selectMany.ToArray());
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

public static class IOMonadExtensions
{
    public static IO<R> Select<T, R>(this IO<T> ioMonad, Func<T, R> selector)
    {
        return new IO<R>(() =>
        {
            var result = ioMonad.Run();
            return selector(result);
        });
    }

    public static IO<R> SelectMany<T, R>(this IO<T> ioMonad, Func<T, IO<R>> selector)
    {
        return new IO<R>(() =>
        {
            var result = ioMonad.Run();
            return selector(result).Run();
        });
    }

    public static IO<R> SelectMany<T, U, R>(
        this IO<T> ioMonad,
        Func<T, IO<U>> selector,
        Func<T, U, R> resultSelector)
    {
        return new IO<R>(() =>
        {
            var result = ioMonad.Run();
            var innerMonad = selector(result);
            var innerResult = innerMonad.Run();
            return resultSelector(result, innerResult);
        });
    }
}
