using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.Notifications
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

        private ContainerBuilder ConfigureAutofac()
        {
            var builder = new ContainerBuilder();
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder
                .Register<ServiceFactory>(context =>
                    {
                        var c = context.Resolve<IComponentContext>();
                        return t => c.Resolve(t);
                    });

            builder
                .RegisterAssemblyTypes(typeof(PingNotificationHandler1).GetTypeInfo().Assembly).AsImplementedInterfaces();
            //builder.RegisterType<PingHandler>().AsImplementedInterfaces().InstancePerDependency();          // or individually

            return builder;
        }

        [Fact]
        async void use_ping()
        {
            var mediator = _scope.Resolve<IMediator>();

            await mediator.Publish(new PingNotification());

            messages.Should().Contain("one");
            messages.Should().Contain("two");
        }
        
        internal static readonly List<string> messages = new();
    }

    internal class PingNotificationHandler1 : INotificationHandler<PingNotification>
    {
        Task INotificationHandler<PingNotification>.Handle(PingNotification notification, CancellationToken cancellationToken)
        {
            With.messages.Add("one");
            return Task.CompletedTask;
        }
    }
    
    // ReSharper disable once UnusedType.Global
    internal class PingNotificationHandler2 : INotificationHandler<PingNotification>
    {
        Task INotificationHandler<PingNotification>.Handle(PingNotification notification, CancellationToken cancellationToken)
        {
            With.messages.Add("two");
            return Task.CompletedTask;
        }
    }

    // Cannot be internal; can be nested
    public class PingNotification : INotification
    {
        
    }

}
