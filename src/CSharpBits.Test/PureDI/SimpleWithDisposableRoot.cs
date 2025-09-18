using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static CSharpBits.Test.PureDI.SimpleCompositionRootWithDisposableRoot;

namespace CSharpBits.Test.PureDI;

internal static class DisposableExtensions
{
    internal static void DisposeOfAll(this List<IDisposable> disposables) => disposables.ForEach(s => s.Dispose());
}

file class Scope : IDisposable
{
    private List<IDisposable> _disposables;

    internal Level1 Create()
    {
        Func<InnerScope> innerScopeFactory = () => new();

        var level1 = new Level1(innerScopeFactory);

        _disposables = [level1];

        return level1;
    }

    public void Dispose()
    {
        _disposables.DisposeOfAll();
    }
}

file class InnerScope : IDisposable
{
    private List<IDisposable> _disposables;

    internal Inner1 Create()
    {
        var inner2 = new Inner2();
        var inner1 = new Inner1(inner2);

        _disposables = [inner1, inner2];

        return inner1;
    }

    public void Dispose()
    {
        _disposables.DisposeOfAll();
    }
}

file record Level1 : IDisposable
{
    private readonly Func<InnerScope> _innerScopeFactory;

    public Level1(Func<InnerScope> InnerScopeFactory)
    {
        Log("new Level1");
        _innerScopeFactory = InnerScopeFactory;
    }

    void IDisposable.Dispose()
    {
        Log("Disposing of Level1");
    }

    internal void DoJob()
    {
        Enumerable.Range(1, 5).ToList().ForEach(_ =>
            {
                using var scope = _innerScopeFactory();
                var inner1 = scope.Create();
                inner1.DoJob();
            }
        );
    }
}

file record Inner1(
    Inner2 Inner2) : IDisposable
{
    void IDisposable.Dispose() => Log("- Inner1");

    internal void DoJob()
    {
        Inner2.DoJob();
        Log("job Inner1");
    }
}

file record Inner2 : IDisposable
{
    internal Inner2()
    {
        Log("+ Inner2");
    }
    void IDisposable.Dispose()
    {
        Log("- Inner2");
    }

    internal void DoJob() => Log("job Inner2");
}

public class SimpleCompositionRootWithDisposableRoot
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
                "new Level1",

                    "+ Inner2",
                    "job Inner2",
                    "job Inner1",
                    "- Inner1",
                    "- Inner2",

                    "+ Inner2",
                    "job Inner2",
                    "job Inner1",
                    "- Inner1",
                    "- Inner2",

                    "+ Inner2",
                    "job Inner2",
                    "job Inner1",
                    "- Inner1",
                    "- Inner2",

                    "+ Inner2",
                    "job Inner2",
                    "job Inner1",
                    "- Inner1",
                    "- Inner2",

                    "+ Inner2",
                    "job Inner2",
                    "job Inner1",
                    "- Inner1",
                    "- Inner2",

                "Disposing of Level1"
                ];
        Assert.Equal(expected, Messages);
    }
}
