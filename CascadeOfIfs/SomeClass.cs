using System.Collections.Generic;
using System.Linq;
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

            return operations.ChainTogether(_check).Execute();
        }
    }

    internal static class OperationsExtensions
    {
        internal static IRing ChainTogether(this IEnumerable<IOperation> operations, Check check) =>
            operations.Reverse()
                .Select(o => new OperationRing(check, o))
                .Aggregate((IRing) new PassAll(), (previous, ring) =>
                {
                    ring.Next = previous;
                    return ring;
                });
    }

    internal enum HResult
    {
        Operation1Failed,
        Operation2Failed,
        Operation3Failed,
        Operation4Failed,
        Ok
    }
}