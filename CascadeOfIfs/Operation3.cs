namespace CascadeOfIfs
{
    internal class Operation3 : IOperation {
        public string DoJob()
        {
            return "operation3 result";
        }
        public HResult ErrorCode => HResult.Operation3Failed;
    }
}