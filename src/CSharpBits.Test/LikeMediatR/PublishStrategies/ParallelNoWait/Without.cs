using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Xunit;
using static CSharpBits.Test.LikeMediatR.PublishStrategies.ParallelNoWait.Without;

namespace CSharpBits.Test.LikeMediatR.PublishStrategies.ParallelNoWait
{
    public class Without : IDisposable
    {
        private readonly IContainer _container;
        private readonly ILifetimeScope _scope;

        public Without()
        {
            var builder = ConfigureAutofac();

            _container = builder.Build();
            _scope = _container.BeginLifetimeScope();
        }

        void IDisposable.Dispose()
        {
            _scope.Dispose();
            _container.Dispose();
        }

        private static ContainerBuilder ConfigureAutofac()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterType<PlainWillRaiseException>().As<IPingHandler>();
            builder
                .RegisterType<PlainPingNotificationHandler>().As<IPingHandler>();
            return builder;
        }

        [Fact]
        async Task use_ping()
        {
            var pingHandlers = _scope.Resolve<IEnumerable<IPingHandler>>();

            await pingHandlers.RunNoWait(h => h.Ping());

            Messages.Should().Contain("one");
            Messages.Should().Contain("two");
        }

        internal static readonly List<string> Messages = new();
    }


    internal interface IPingHandler
    {
        Task Ping();
    }

    internal class PlainWillRaiseException : IPingHandler
    {
        async Task IPingHandler.Ping()
        {
            Messages.Add("one");
            throw new ArgumentException("Intentionally failing");
        }
    }

    internal class PlainPingNotificationHandler : IPingHandler
    {
        async Task IPingHandler.Ping()
        {
            Messages.Add("two");
        }
    }

    internal static class CompositionExtensions
    {
        internal static Task RunNoWait<T>(this IEnumerable<T> handlers, Func<T, Task> action)
        {
            handlers.Select(action).ToList();

            return Task.CompletedTask;
        }
    }
}
