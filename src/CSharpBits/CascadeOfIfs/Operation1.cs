namespace CSharpBits.CascadeOfIfs;

internal class Operation1 : IOperation {
    public string DoJob()
    {
        return "operation1 result";
    }

    public HResult ErrorCode => HResult.Operation1Failed;
}