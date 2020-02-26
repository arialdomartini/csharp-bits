using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser
{
    internal class State
    {
        private readonly string _name;
        private readonly Destination _destination;

        private State(string name, Destination destination = default)
        {
            _name = name;
            _destination = destination;
        }

        public Either<string, StateResult> Eval(string unknown, Result result)
        {
            if (unknown != _destination.Message) return "error";

            return StateResult.Success(_destination.To, result.Track(_name));
        }

        internal static State Empty(string name) => new State(name);

        internal static State From(string name, Destination destination) => new State(name, destination);
    }
}