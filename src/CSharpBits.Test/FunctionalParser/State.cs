using System.Linq;
using Pie.Monads;

namespace CSharpBits.Test.FunctionalParser
{
    internal class State
    {
        private readonly string _name;
        private readonly Destination[] _destinations;

        private State(string name, params Destination[] destinations)
        {
            _name = name;
            _destinations = destinations;
        }

        public Either<string, StateResult> Eval(string message, Result result)
        {
            var destination = _destinations.SingleOrDefault(d => d.CanHandle(message));

            if(destination.Equals(default(Destination))) return "error";

            return StateResult.Success(destination.To, result.Track(_name));
        }

        internal static State Empty(string name) => new State(name);

        internal static State From(string name, params Destination[] destinations) => new State(name, destinations);
    }
}