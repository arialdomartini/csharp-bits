namespace CascadeOfIfs
{
    internal class TestClass
    {
        HResult SomeFunction()
        {
            var result = HResult.Ok;

            if(Succeeded(Operation1()))
            {
                if(Succeeded(Operation2()))
                {
                    if(Succeeded(Operation3()))
                    {
                        if(Succeeded(Operation4()))
                        {
                        }
                        else
                        {
                            result = HResult.Operation4Failed;
                        }
                    }
                    else
                    {
                        result = HResult.Operation3Failed;
                    }
                }
                else
                {
                    result = HResult.Operation2Failed;
                }
            }
            else
            {
                result = HResult.Operation1Failed;
            }

            return result;
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