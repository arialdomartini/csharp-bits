using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.FunctionalParser
{
    public class FunctionalParserTest
    {
        private const string Message = "message";

        [Fact]
        void single_state_fails()
        {
            var to = State.Empty("2");
            var emptyState = State.From("1", Message, to);

            var result = emptyState.Eval("unknown", new Result());

            result.Left().Should().Be("error");
        }

        [Fact]
        void single_state_move_to_next_state()
        {
            var to = State.Empty("2");
            var from = State.From("1", Message, to);

            var result = from.Eval(Message, new Result()).Right();

            result.State.Should().Be(to);
            result.Result.Tracks.Should().BeEquivalentTo("1");
        }
    }
}