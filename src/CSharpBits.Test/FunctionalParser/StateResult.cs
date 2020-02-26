namespace CSharpBits.Test.FunctionalParser
{
    internal class StateResult
    {
        internal State State { get; set; }
        internal Result Result { get; set; }

        internal static StateResult Success(State state, Result result) =>
            new StateResult {State = state, Result = result};
    }
}