﻿using FluentAssertions;
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
            var sut = new SomeClass {FailWhen = failWhen};

            var result = sut.SomeFunction();

            result.Should().Be(expectedResult);
        }
    }

    public class SomeClass
    {
        internal string FailWhen = "some condition";

        internal HResult SomeFunction()
        {
            if (!Succeeded(Operation1()))
                return HResult.Operation1Failed;

            if (!Succeeded(Operation2()))
                return HResult.Operation2Failed;

            if (!Succeeded(Operation3()))
                return HResult.Operation3Failed;

            if (!Succeeded(Operation4()))
                return HResult.Operation4Failed;

            return HResult.Ok;
        }

        private string Operation1()
        {
            // some operations
            return "operation1 result";
        }

        private string Operation2()
        {
            // some operations
            return "operation2 result";
        }

        private string Operation3()
        {
            // some operations
            return "operation3 result";
        }

        private string Operation4()
        {
            // some operations
            return "operation4 result";
        }

        private bool Succeeded(string operationResult) =>
            operationResult != FailWhen;
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