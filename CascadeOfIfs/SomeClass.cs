using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace CascadeOfIfs
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
            var sut = new SomeClass(new Check(failWhen));

            var result = sut.SomeFunction();

            result.Should().Be(expectedResult);
        }
    }

    internal interface IOperation
    {
        string DoJob();
        HResult ErrorCode { get; }
    }

    internal class SomeClass
    {
        private readonly Check _check;

        public SomeClass(Check check)
        {
            _check = check;
        }

        internal HResult SomeFunction()
        {
            var operations = new List<IOperation>
            {
                new Operation1(),
                new Operation2(),
                new Operation3(),
                new Operation4()
            };

            foreach (var operation in operations)
            {
                if (!_check.Succeeded(operation.DoJob()))
                    return operation.ErrorCode;
            }

            return HResult.Ok;
        }
    }

    internal class Check
    {
        private readonly string _failWhen;

        public Check(string failWhen)
        {
            _failWhen = failWhen;
        }

        internal bool Succeeded(string operationResult) =>
            operationResult != _failWhen;
    }

    internal enum HResult
    {
        Ok,
        Operation1Failed,
        Operation2Failed,
        Operation3Failed,
        Operation4Failed,
    }
}