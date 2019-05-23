namespace CascadeOfIfs
{
    internal class TestClass
    {
        HResult SomeFunction()
        {
            if(Succeeded(Operation1()))
            {
                if(Succeeded(Operation2()))
                {
                    if(Succeeded(Operation3()))
                    {
                        if(Succeeded(Operation4()))
                        {
                            return HResult.Ok;
                        }
                        else
                        {
                            return HResult.Operation4Failed;
                        }
                    }
                    else
                    {
                        return HResult.Operation3Failed;
                    }
                }
                else
                {
                    return HResult.Operation2Failed;
                }
            }
            else
            {
                return HResult.Operation1Failed;
            }
        }

        private string Operation1()
        {
            // some operations
            return "operation1 result";
        }
        private string Operation2()
        {
            // some operations
            return "operation2 result";
        }
        private string Operation3()
        {
            // some operations
            return "operation3 result";

        }
        private string Operation4()
        {
            // some operations
            return "operation4 result";

        }

        private bool Succeeded(string operationResult) =>
            operationResult == "some condition";
    }

    internal enum HResult
    {
        Ok,
        Operation1Failed,
        Operation2Failed,
        Operation3Failed,
        Operation4Failed,
    }
}