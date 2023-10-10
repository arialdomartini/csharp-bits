using System;
using System.IO;
using System.Text;
using Xunit;

namespace CSharpBits.Test.MonadsForTheRestOfsUs;

public class IoMonadTest
{
    class StubConsole : IDisposable
    {
        private readonly StringBuilder _sb;
        internal string Output => _sb.ToString();
        private readonly TextWriter _originalOutput;

        internal StubConsole()
        {
            _originalOutput = Console.Out;    
            _sb = new StringBuilder();
            Console.SetOut(new StringWriter(_sb));
        }
        void IDisposable.Dispose()
        {
            Console.SetOut(_originalOutput);
        }
    }
    
    [Fact]
    void an_impure_function()
    {
        var originalOutput = Console.Out;
        
        int CalculateWithSideEffect(string s)
        {
            Console.Write("I'm a side effect!");
            return s.Length;
        }

        var sb = new StringBuilder();
        Console.SetOut(new StringWriter(sb));

        var length = CalculateWithSideEffect("foo");
        
        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", sb.ToString());

        Console.SetOut(originalOutput);
    }

    [Fact]
    void an_impure_function_on_file()
    {
        int CalculateWithSideEffect(string s)
        {
            File.WriteAllText("output.txt", "I'm a side effect!");
            return s.Length;
        }

        var length = CalculateWithSideEffect("foo");
        
        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    record IO<T>(T value, Action action);
    
    [Fact]
    void a_pure_IO_monadic()
    {
        // IO<int> CalculateWithSideEffect(string s)
        // {
        //     return new IO<int>(
        //         s.Length,
        //         () => File.WriteAllText("output.txt", "I'm a side effect!"));
        // }
        //
        // IO<int> length = CalculateWithSideEffect("foo");
        //
        // Assert.Equal(3, length);
        // Assert.Equal("I'm a side effect!", File.ReadAllText("output.txt"));
    }

    [Fact]
    void an_impure_function_with_stub()
    {
        using var stub = new StubConsole();
        
        int CalculateWithSideEffect(string s)
        {
            Console.Write("I'm a side effect!");
            return s.Length;
        }
        var length = CalculateWithSideEffect("foo");
        
        Assert.Equal(3, length);
        Assert.Equal("I'm a side effect!", stub.Output);
    }
    
    [Fact]
    void composition_of_pure_functions()
    {
        int CalculateWithSideEffect(string s)
        {
            Console.Write("I'm a side effect!");
            return s.Length;
        }

        int Double(int i) => i * 2;

        var doubleTheLength = Double(CalculateWithSideEffect("foo"));
        
        Assert.Equal(6, doubleTheLength);
    }
    
}
