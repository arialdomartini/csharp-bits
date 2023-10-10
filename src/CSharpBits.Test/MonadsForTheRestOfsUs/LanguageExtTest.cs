using System;
using LanguageExt;
using Xunit;

namespace CSharpBits.Test.SomethingElse;



public class LanguageExtTest
{
    private static Either<string, int> Computation1()
    {
        throw new Exception();
    }
    
    private static Either<string, int> Computation2(int i)
    {
        throw new Exception();
    }
    
    private static Either<string, int> Computation3(int value2, int i)
    {
        throw new Exception();
    }

    [Fact]
    void with_io()
    {
        Either<string, int> result =
            from value1 in Computation1()
            from value2 in Computation2(value1)
            from value3 in Computation3(value2, value1)
            select value2;
    }
}
