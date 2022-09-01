using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.Notifications
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

        private ContainerBuilder ConfigureAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PlainPingNotificationHandler1>().As<IPingNotificationHandler>().InstancePerLifetimeScope();
            builder.RegisterType<PlainPingNotificationHandler2>().As<IPingNotificationHandler>().InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var handlers = c.Resolve<IEnumerable<IPingNotificationHandler>>();
                    return new Composite(handlers);
                })
                .Named<IPingNotificationHandler>("composite").InstancePerLifetimeScope();

            return builder;
        }

        [Fact]
        async void use_ping()
        {
            var composite = _scope.ResolveNamed<IPingNotificationHandler>("composite");

            await composite.Ping();

            messages.Should().Contain("one");
            messages.Should().Contain("two");
        }

        internal static readonly List<string> messages = new();
    }

    internal class Composite : IPingNotificationHandler
    {
        private readonly IEnumerable<IPingNotificationHandler> _handlers;

        internal Composite(IEnumerable<IPingNotificationHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task Ping()
        {
            foreach (var handler in _handlers) await handler.Ping();
        }
    }

    internal class PlainPingNotificationHandler1 : IPingNotificationHandler
    {
        Task IPingNotificationHandler.Ping()
        {
            Without.messages.Add("one");
            return Task.CompletedTask;
        }
    }

    internal class PlainPingNotificationHandler2 : IPingNotificationHandler
    {
        Task IPingNotificationHandler.Ping()
        {
            Without.messages.Add("two");
            return Task.CompletedTask;
        }
    }

    internal interface IPingNotificationHandler
    {
        Task Ping();
    }
}
