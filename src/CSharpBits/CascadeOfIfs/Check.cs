namespace CSharpBits.CascadeOfIfs
{
    internal class Check
    {
        private readonly string _failWhen;

        public Check(string failWhen)
        {
            _failWhen = failWhen;
        }

        internal bool Succeeded(string operationResult) =>
            operationResult != _failWhen;
    }
}