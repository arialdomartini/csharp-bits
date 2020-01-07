namespace CSharpBits.CascadeOfIfs
{
    internal class Operation2 : IOperation {
        public string DoJob()
        {
            return "operation2 result";
        }
        public HResult ErrorCode => HResult.Operation2Failed;
    }
}