using FluentAssertions;
using Pie.Monads;
using Xunit;

namespace CSharpBits.Test.FunctionalParser
{
    public class FunctionalParserTest
    {
        [Fact]
        void single_state_fails()
        {
            var state = new State();

            var result = state.Eval("unknown");

            result.IsLeft().Should().Be(true);
        }
    }

    internal class State
    {
        public Either<string, State> Eval(string unknown)
        {
            return "error";
        }
    }
}