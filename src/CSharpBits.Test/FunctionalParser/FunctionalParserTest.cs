using FluentAssertions;
using Pie.Monads;
using Xunit;

namespace CSharpBits.Test.FunctionalParser
{
    public class FunctionalParserTest
    {
        private const string Message = "message";

        [Fact]
        void single_state_fails()
        {
            var to = State.Empty();
            var emptyState = State.From(Message, to);

            var result = emptyState.Eval("unknown");

            result.Left().Should().Be("error");
        }

        [Fact]
        void single_state_move_to_next_state()
        {
            var to = State.Empty();
            var from = State.From(Message, to);

            var result = from.Eval(Message).Right();

            result.Should().Be(to);
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