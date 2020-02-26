using FluentAssertions;
using Pie.Monads;
using Xunit;

namespace CSharpBits.Test.FunctionalParser
{
    internal static class StateAssertions
    {
        internal static void ShouldBeRight<L, R>(this Either<L, R> either) => either.IsRight().Should().Be(true);
        internal static void ShouldBeLeft<L, R>(this Either<L, R> either) => either.IsLeft().Should().Be(true);
    }

    public class FunctionalParserTest
    {
        private const string Message = "message";

        [Fact]
        void single_state_fails()
        {
            var emptyState = State.Empty();

            var result = emptyState.Eval("unknown");

            result.ShouldBeLeft();
        }

        [Fact]
        void single_state_move_to_next_state()
        {
            var to = State.Empty();
            var from = State.From(Message, to);

            var result = from.Eval(Message);

            result.ShouldBeRight();
        }
    }

    internal class State
    {
        private readonly string _message;
        private readonly State _to;

        private State() : this("", null) {}

        private State(string message, State to)
        {
            _message = message;
            _to = to;
        }

        public Either<string, State> Eval(string unknown)
        {
            if (unknown == _message) return _to;
            return "error";
        }

        internal static State Empty() => new State();

        internal static State From(string message, State to) => new State(message, to);
    }
}