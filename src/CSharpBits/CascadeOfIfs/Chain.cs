using System.Collections.Generic;

namespace CSharpBits.CascadeOfIfs
{
    internal class Chain
    {
        private readonly Check _check;
        private List<Ops> _ops = new List<Ops>();

        public Chain(Check check)
        {
            _check = check;
        }

        public void Append(IOperation operation, HResult errorCode)
        {
            _ops.Add(new Ops {Operation = operation, Result = errorCode});
        }
    }

    internal class Ops
    {
        public IOperation Operation { get; set; }
        public HResult Result { get; set; }
        public Ops Next { get; set; }
    }
}