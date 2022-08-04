using System;

namespace CrashingModule
{
    class Initializer
    {
        static Initializer() => Console.WriteLine("Initializer.cctor");

        [ModuleInitializer]
        public static void Initialize1() => Console.WriteLine("Module Initializer 1");

        [ModuleInitializer]
        public static void Initialize2() => Console.WriteLine("Module Initializer 2");
    }
}