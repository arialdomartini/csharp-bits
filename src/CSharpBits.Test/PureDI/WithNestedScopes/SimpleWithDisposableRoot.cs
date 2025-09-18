using System.Collections.Generic;
using Xunit;

namespace CSharpBits.Test.PureDI.WithNestedScopes;

public class NestedScopesTest
{
    private static readonly List<string> Messages = new();

    internal static void Log(string message) => Messages.Add(message);

    [Fact]
    void run_all()
    {
        var compositionRoot = new Scope();

        var level1 = compositionRoot.Create();
        level1.DoJob();

        compositionRoot.Dispose();

        List<string> expected =
            [
                "+ GlobalSingleton",
                "new Level1",

                    "+ ScopeSingleton",
                    "+ Inner2",
                    "job GlobalSingleton",
                    "job Inner1",
                    "job Inner2",
                    "job GlobalSingleton",
                    "- Inner1",
                    "- Inner2",
                    "- ScopeSingleton",

                    "+ ScopeSingleton",
                    "+ Inner2",
                    "job GlobalSingleton",
                    "job Inner1",
                    "job Inner2",
                    "job GlobalSingleton",
                    "- Inner1",
                    "- Inner2",
                    "- ScopeSingleton",

                    "+ ScopeSingleton",
                    "+ Inner2",
                    "job GlobalSingleton",
                    "job Inner1",
                    "job Inner2",
                    "job GlobalSingleton",
                    "- Inner1",
                    "- Inner2",
                    "- ScopeSingleton",

                "Disposing of Level1",
                "- GlobalSingleton"
                ];
        Assert.Equal(expected, Messages);
    }
}
