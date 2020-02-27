namespace CSharpBits.Test.FunctionalParser
{
    internal class StateResult
    {
        internal State State { get; private set; }
        internal Result Result { get; private set; }

        internal static StateResult Success(State state, Result result) =>
            new StateResult {State = state, Result = result};
    }
}