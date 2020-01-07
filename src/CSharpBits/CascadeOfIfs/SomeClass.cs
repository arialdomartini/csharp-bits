using System.Collections.Generic;

namespace CSharpBits.CascadeOfIfs
{
    internal class SomeClass
    {
        private readonly Check _check;
        private readonly List<IOperation> _operations;

        public SomeClass(Check check, List<IOperation> operations)
        {
            _check = check;
            _operations = operations;
        }

        internal HResult SomeFunction()
        {
            return _operations
                .ChainTogether(_check)
                .Execute();
        }
    }
}