using System;
using System.Runtime.CompilerServices;

namespace CSharpBits.Test
{
    class Initializer
    {
        [ModuleInitializer]
        public static void Crash() =>
            throw new Exception("Class Foo does not have an empty constructor");
    }
}