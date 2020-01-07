using System.Collections.Generic;
using Xunit;
using CSharpBits.CascadeOfIfs;
using FluentAssertions;

namespace CSharpBits.Test.CascadeOfIfs
{
    public class TestCase
    {
        [Theory]
        [InlineData("operation1 result", HResult.Operation1Failed)]
        [InlineData("operation2 result", HResult.Operation2Failed)]
        [InlineData("operation3 result", HResult.Operation3Failed)]
        [InlineData("operation4 result", HResult.Operation4Failed)]
        [InlineData("never", HResult.Ok)]
        void acceptance_test(string failWhen, HResult expectedResult)
        {
            var operations = new List<IOperation>
            {
                new Operation1(),
                new Operation2(),
                new Operation3(),
                new Operation4()
            };

            var check = new Check(failWhen);

            var sut = new SomeClass(check, operations);

            var result = sut.SomeFunction();

            result.Should().Be(expectedResult);
        }
    }
}