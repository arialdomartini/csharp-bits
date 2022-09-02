using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CSharpBits.Test.LikeMediatR.Notifications;
using FluentAssertions;
using MediatR;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.GenericHandling
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
                .RegisterAssemblyTypes(typeof(GenericNotificationHandler).GetTypeInfo().Assembly).AsImplementedInterfaces();

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
            
            builder.RegisterType<GenericNotificationHandler>()
                .As<INotificationHandler<PingNotification>>();
            
            return builder;
        }

        [Fact]
        async void invokes_the_generic_notification_handler()
        {
            var mediator = _scope.Resolve<IMediator>();

            await mediator.Publish(new PingNotification());

            messages.Should().Contain("one");
            messages.Should().Contain("two");
            messages.Should().Contain("generic");
        }
        
        internal static readonly List<string> messages = new();
    }

    // ReSharper disable once UnusedType.Global
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
    
    internal class GenericNotificationHandler : INotificationHandler<INotification>
    {
        Task INotificationHandler<INotification>.Handle(INotification notification, CancellationToken cancellationToken)
        {
            With.messages.Add("generic");
            return Task.CompletedTask;
        }
    }

    // Cannot be internal; can be nested
    public class PingNotification : INotification { }
}
