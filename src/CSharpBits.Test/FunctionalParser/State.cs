using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser
{
    internal class State
    {
        private readonly string _message;
        private readonly State _to;
        private string _name;

        private State(string name) : this(name, "", null)
        {
        }

        private State(string name, string message, State to)
        {
            _name = name;
            _message = message;
            _to = to;
        }

        public Either<string, StateResult> Eval(string unknown, Result result)
        {
            if (unknown != _message) return "error";

            return StateResult.Success(_to, result.Track(_name));
        }

        internal static State Empty(string name) => new State(name);

        internal static State From(string name, string message, State to) => new State(name, message, to);
    }
}