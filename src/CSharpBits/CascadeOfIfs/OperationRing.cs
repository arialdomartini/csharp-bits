namespace CSharpBits.CascadeOfIfs
{
    internal class OperationRing : IRing
    {
        private readonly Check _check;
        private readonly IOperation _operation;
        internal IRing Next { private get; set; }

        public OperationRing(Check check, IOperation operation)
        {
            _check = check;
            _operation = operation;
        }

        public HResult Execute()
        {
            var operationResult = _operation.DoJob();

            if (_check.Succeeded(operationResult))
                return Next.Execute();

            return _operation.ErrorCode;
        }
    }
}