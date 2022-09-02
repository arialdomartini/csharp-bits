using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Xunit;
using static CSharpBits.Test.LikeMediatR.PublishStrategies.Generic.Without;

namespace CSharpBits.Test.LikeMediatR.PublishStrategies.Generic
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
            
            Messages = new();
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
        async Task when_all()
        {
            var pingHandlers = _scope.Resolve<IEnumerable<IPingHandler>>();

            await pingHandlers.Run(h => h.Ping(), Task.WhenAll);

            Messages.Should().Contain("one");
            Messages.Should().Contain("two");
        }
        
        [Fact]
        async Task when_any()
        {
            var pingHandlers = _scope.Resolve<IEnumerable<IPingHandler>>();

            await pingHandlers.Run(h => h.Ping(), Task.WhenAny);

            Messages.Should().Contain("one");
        }

        internal static List<string> Messages;
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
        }
    }

    internal class PlainPingNotificationHandler : IPingHandler
    {
        async Task IPingHandler.Ping()
        {
            await Task.Delay(50);
            Messages.Add("two");
        }
    }

    internal static class CompositionExtensions
    {
        internal static Task Run<T>(this IEnumerable<T> handlers, Func<T, Task> action, Func<IEnumerable<Task>, Task> then) =>
            then(handlers.Select(action).ToList());
    }
}
