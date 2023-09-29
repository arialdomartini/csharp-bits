namespace CSharpBits.CascadeOfIfs;

internal interface IOperation
{
    string DoJob();
    HResult ErrorCode { get; }
}