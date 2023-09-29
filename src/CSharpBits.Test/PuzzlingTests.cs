using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using static System.Environment;
using static CSharpBits.Test.EnumerableDependencyInjectionHelper;
using static CSharpBits.Test.TestHelper;

namespace CSharpBits.Test;

class EnumerableDependencyInjectionHelper
{
    internal static IEnumerable<T> DisguiseAsIEnumerable<T, A, B>(Func<A, B> f) =>
        f.DisguiseAsIEnumerable<T, A, B>();
}
static class EnumerableDependencyInjection
{
    class EnumerableException<A, B>: Exception
    {
        internal Func<A, B> Function { get; set; }
    }

    internal static void Execute<T>(this IEnumerable<T> enumerable) =>
        enumerable.Aggregate((_, v) => v);


    internal static B Execute<T, A, B>(this IEnumerable<T> enumerable, A argument)
    {
        try
        {
            enumerable.Aggregate((_, v) => v);
            return default; // this is unreachable code
        }
        catch (EnumerableException<A,B> e)
        {
            return e.Function(argument);
        }
    }

    internal static IEnumerable<T> DisguiseAsIEnumerable<T>(this Action a)
    {
        IEnumerable<T> arbitraryCode()
        {
            yield return default(T);
            a();
        }

        return arbitraryCode();
    }

    internal static IEnumerable<T> DisguiseAsIEnumerable<T, A, B>(this Func<A, B> f)
    {
        IEnumerable<T> arbitraryCode()
        {
            yield return default(T);
            f.Throw();
        }

        return arbitraryCode();
    }

    private static void Throw<A, B>(this Func<A, B> f) =>
        throw new EnumerableException<A, B> { Function = f };
}

public class DependencyInjectionViaIEnumerable
{
    class MyTargetClass
    {
        private readonly IEnumerable<int> _dependency;

        internal MyTargetClass(IEnumerable<int> dependency) =>
            _dependency = dependency;

        internal void SomeMethod() =>
            _dependency.Execute();
    }


    [Fact]
    void dependency_injection_of_an_action_with_IEnumerable()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);

        void ToBeInjected()
        {
            Console.WriteLine("It worked");
        }

        IEnumerable<int> dependencyAsIEnumerable =
            ((Action)ToBeInjected).DisguiseAsIEnumerable<int>();

        new MyTargetClass(dependencyAsIEnumerable)
            .SomeMethod();

        Assert.Equal($"It worked{NewLine}", writer.GetStringBuilder().ToString());
    }

    class MyTargetClass2
    {
        private readonly IEnumerable<decimal> _dependency;

        internal MyTargetClass2(IEnumerable<decimal> dependency) =>
            _dependency = dependency;

        internal int SomeMethod(string argument) =>
            _dependency.Execute<decimal, string, int>(argument);
    }

    [Fact]
    void dependency_injection_of_a_function_with_IEnumerable()
    {
        int ToBeInjected(string s)
        {
            return s.Length;
        }

        IEnumerable<decimal> decimals =
            DisguiseAsIEnumerable<decimal, string, int>(ToBeInjected);

        var result = new MyTargetClass2(decimals)
            .SomeMethod("abc");

        Assert.Equal(3, result);
    }

}


public class PuzzlingTest
{
    class TargetClass
    {
        internal void SomeMethod(IEnumerable<int> numbers)
        {
            // [...]
            numbers.ToList();
            // [...]
        }
    }

    string testResult = "arbitraryCode() hasn't been run";

    [Fact]
    void function_injection_via_IEnumerable()
    {
        void ToBeInjected()
        {
            testResult = "Hey! arbitraryCode() was executed";
        }

        IEnumerable<int> arbitraryCode()
        {
            yield return 0;
            ToBeInjected();
        }

        IEnumerable<int> looksLikeNumbers = arbitraryCode();
        Assert.Equal("arbitraryCode() hasn't been run", testResult);

        new TargetClass()
            .SomeMethod(looksLikeNumbers);

        testResult.Should().Be("Hey! arbitraryCode() was executed");
    }


    [Fact]
    public void intermittent_test2()
    {
        IEnumerable<string> strings = SomeTexts;
        Assert.Equal(new[] { "foo", "bar", "baz" }, strings.ToArray());

        var toUpper1 = strings.Select(s => s.ToUpper());
        Assert.Equal(new[] { "FOO", "BAR", "BAZ" }, toUpper1.ToArray());


        "".Invoking(_ =>
            {
                strings.Select(s => s.ToUpper() + "")
                    .ToArray();
            }).Should()
            .Throw<PlatformNotSupportedException>();


        var toUpper3 = strings.Select(s => "" + s.ToUpper());
        Assert.Equal(new[] { "FOO", "BAR", "BAZ" }, toUpper3.ToArray());
    }


    [Fact]
    public void intermittent_test()
    {
        IEnumerable<string> strings = SomeStrings;
        Assert.Equal(new[] { "Foo", "Bar", "Baz" }, strings.ToArray());

        var test1 = strings.Select(s => new string(s.ToCharArray()));
        Assert.Equal(new[] { "Foo", "Bar", "Baz" }, test1.ToArray());


        var test2 = strings.Select(s => new string(s.ToCharArray()) + "");
        Assert.Equal(new[]
        {
            "Argument Foo of type System.String is not assignable to parameter type int32",
            "Argument Bar of type System.String is not assignable to parameter type int32",
            "Constructor 'Baz' has 0 parameter(s) but is invoked with 0 argument(s)"
        }, test2.ToArray());

        var test3 = strings.Select(s => "" + new string(s.ToCharArray()));
        Assert.Equal(new[] { "Foo", "Bar", "Baz" }, test3.ToArray());
    }
}

internal static class TestHelper
{
    private static int count1;

    internal static IEnumerable<string> SomeTexts
    {
        get
        {
            count1++;
            if (count1 == 1 || count1 % 2 == 0)
            {
                yield return "foo";
                yield return "bar";
                yield return "baz";

                yield break;
            }

            throw new PlatformNotSupportedException();
        }
    }

    private static int count2;

    internal static IEnumerable<string> SomeStrings
    {
        get
        {
            count2++;
            if (count2 == 1 || count2 % 2 == 0)
            {
                yield return "Foo";
                yield return "Bar";
                yield return "Baz";
            }
            else
            {
                yield return "Argument Foo of type System.String is not assignable to parameter type int32";
                yield return "Argument Bar of type System.String is not assignable to parameter type int32";
                yield return "Constructor 'Baz' has 0 parameter(s) but is invoked with 0 argument(s)";
            }
        }
    }
}