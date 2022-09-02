using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.PublishStrategies.ParallelNoWait
{
    public class With : IDisposable
    {
        private readonly IContainer _container;
        private readonly ILifetimeScope _scope;

        public With()
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

        private Task ParallelNoWait(IEnumerable<Func<INotification, CancellationToken, Task>> handlers, INotification notification,
            CancellationToken cancellationToken)
        {
            foreach (var handler in handlers)
            {
                Task.Run(() => handler(notification, cancellationToken));
            }

            return Task.CompletedTask;
        }

        private ContainerBuilder ConfigureAutofac()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterAssemblyTypes(typeof(WillRaiseException).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder
                .Register(c =>
                {
                    var serviceFactory = c.Resolve<ServiceFactory>();
                    return new CustomMediator(serviceFactory, ParallelNoWait);
                })
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder
                .Register<ServiceFactory>(context =>
                {
                    var c = context.Resolve<IComponentContext>();
                    return t => c.Resolve(t);
                });

            return builder;
        }

        [Fact]
        async Task use_ping()
        {
            var mediator = _scope.Resolve<IMediator>();

            await mediator.Publish(new PingNotification());

            messages.Should().Contain("one");
            messages.Should().Contain("two");
        }

        internal static readonly List<string> messages = new();
    }

    public class CustomMediator : Mediator
    {
        private readonly Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> _publish;

        public CustomMediator(
            ServiceFactory serviceFactory,
            Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> publish) : base(serviceFactory)
        {
            _publish = publish;
        }

        protected override Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification,
            CancellationToken cancellationToken)
        {
            return _publish(allHandlers, notification, cancellationToken);
        }
    }


    internal class WillRaiseException : INotificationHandler<PingNotification>
    {
        Task INotificationHandler<PingNotification>.Handle(PingNotification notification, CancellationToken cancellationToken)
        {
            With.messages.Add("one");
            throw new ArgumentException("Intentionally failing");
        }
    }

    // ReSharper disable once UnusedType.Global
    internal class PingNotificationHandler : INotificationHandler<PingNotification>
    {
        Task INotificationHandler<PingNotification>.Handle(PingNotification notification, CancellationToken cancellationToken)
        {
            With.messages.Add("two");
            return Task.CompletedTask;
        }
    }

    public class PingNotification : INotification
    {
    }
}
