namespace CSharpBits.CascadeOfIfs;

internal class Operation4 : IOperation {
    public string DoJob()
    {
        return "operation4 result";
    }
    public HResult ErrorCode => HResult.Operation4Failed;
}