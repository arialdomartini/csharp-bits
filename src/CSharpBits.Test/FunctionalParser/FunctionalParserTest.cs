using FluentAssertions;
using Pie.Monads;
using Xunit;

namespace CSharpBits.Test.FunctionalParser
{
    public class FunctionalParserTest
    {
        private const string Message = "message";
        private static Either<string, StateResult> Pure(State state) => StateResult.Success(state, new Result());

        [Fact]
        void single_state_fails()
        {
            var to = State
                .Empty("2");
            var emptyState = State.From("1", (Message, to));

            var result = emptyState.Eval("unknown", new Result());

            result.Left().Should().Be("error");
        }

        [Fact]
        void single_state_move_to_next_state()
        {
            var to = State.Empty("2");
            var from = State.From("1", (Message, to));

            var result = from.Eval(Message, new Result()).Right();

            result.State.Should().Be(to);
            result.Result.Tracks.Should().BeEquivalentTo("1");
        }

        [Fact]
        void support_to_multiple_destinations()
        {
            var to = State.Empty("2");
            var from = State.From("1", ("one", to), ("two", to));

            {
                var result = from.Eval("one", new Result()).Right();
                result.Result.Tracks.Should().BeEquivalentTo("1");
            }
            {
                var result = from.Eval("two", new Result()).Right();
                result.Result.Tracks.Should().BeEquivalentTo("1");
            }
        }

        [Fact]
        void longer_chain()
        {
            var state3 = State.Empty("3");
            var state2 = State.From("2", ("message 2", state3));
            var state1 = State.From("1", ("message 1", state2));

            var result =
                Pure(state1)
                    .Bind(s => s.State.Eval("message 1", s.Result))
                    .Bind(s => s.State.Eval("message 2", s.Result))
                    .Right();

            result.Result.Tracks.Should().BeEquivalentTo("1", "2");
        }
    }
}