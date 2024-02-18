using System;
using Autofac;
using Autofac.Features.OwnedInstances;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static CSharpBits.Test.AutofacUnitOfWork.UnitOfWorkTest;

// ReSharper disable ConvertToUsingDeclaration
namespace CSharpBits.Test.AutofacUnitOfWork;

public class UnitOfWorkTest
{
    internal static ITestOutputHelper OutputHelper;

    public UnitOfWorkTest(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    [Fact]
    void owned()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<Client>();
        builder.RegisterType<Repository>();
        builder.RegisterType<DependencyA>();
        builder.RegisterType<DependencyB>().InstancePerLifetimeScope();

        using (var container = builder.Build())
        using (var scope = container.BeginLifetimeScope())
        {
            var client = scope.Resolve<Client>();

            var repo1 = client.DoStuffInAUnitOfWork();
            OutputHelper.WriteLine("");
            var repo2 = client.DoStuffInAUnitOfWork();

            repo1.Should().NotBe(repo2);
        }
    }
}

public class Client
{
    private readonly Func<Owned<Repository>> _unitOfWork;

    public Client(Func<Owned<Repository>> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public string DoStuffInAUnitOfWork()
    {
        using(var unitOfWork = _unitOfWork())
        {
            var repository = unitOfWork.Value;

            return repository.Id.ToString();
        }
    }
}

public class Repository : IDisposable
{
    private readonly DependencyA _dependencyA;
    private readonly DependencyB _dependencyB;
    internal Guid Id = Guid.NewGuid();

    public Repository(DependencyA dependencyA, DependencyB dependencyB)
    {
        _dependencyA = dependencyA;
        _dependencyB = dependencyB;
        OutputHelper.WriteLine($"+Repository:\t{Id}");
    }

    public void Dispose()
    {
        OutputHelper.WriteLine($"-Repository:\t{Id}");
    }
}

public class DependencyA : IDisposable
{
    public DependencyB B { get; }
    private readonly Guid Id = Guid.NewGuid();

    public DependencyA(DependencyB b)
    {
        B = b;
        OutputHelper.WriteLine($"+A:\t{Id}");
    }

    public void Dispose()
    {
        OutputHelper.WriteLine($"-A:\t{Id}");
    }
}

public class DependencyB : IDisposable
{
    private readonly Guid Id = Guid.NewGuid();

    public DependencyB()
    {
        OutputHelper.WriteLine($"+B:\t{Id}");
    }

    public void Dispose()
    {
        OutputHelper.WriteLine($"-B:\t{Id}");
    }
}
