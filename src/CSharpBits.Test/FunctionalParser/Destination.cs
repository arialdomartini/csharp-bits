namespace CSharpBits.Test.FunctionalParser
{
    internal struct Destination
    {
        private readonly string _message;

        public State To { get; }

        private Destination(string message, State to)
        {
            _message = message;
            To = to;
        }

        public static implicit operator Destination((string message, State to) tuple) =>
            new Destination(tuple.message, tuple.to);

        public bool CanHandle(string message) => message == _message;
    }
}