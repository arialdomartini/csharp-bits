namespace CSharpBits.Test.FunctionalParser
{
    internal struct Destination
    {
        public Destination(string message, State to)
        {
            Message = message;
            To = to;
        }

        public string Message { get; private set; }
        public State To { get; private set; }

        public static implicit operator Destination((string message, State to) tuple) => new Destination(tuple.message, tuple.to);
    }
}