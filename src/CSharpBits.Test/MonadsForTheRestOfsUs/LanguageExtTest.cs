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
    
}
