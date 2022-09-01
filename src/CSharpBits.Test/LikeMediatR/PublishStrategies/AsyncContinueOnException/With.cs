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

namespace CSharpBits.Test.LikeMediatR.PublishStrategies.AsyncContinueOnException
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

        private async Task AsyncContinueOnException(IEnumerable<Func<INotification, CancellationToken, Task>> handlers, INotification notification,
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            var exceptions = new List<Exception>();

            foreach (var handler in handlers)
            {
                try
                {
                    tasks.Add(handler(notification, cancellationToken));
                }
                catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
                {
                    exceptions.Add(ex);
                }
            }

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                exceptions.Add(ex);
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
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
                    return new CustomMediator(serviceFactory, AsyncContinueOnException);
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

            try
            {
                await mediator.Publish(new PingNotification());
                
                true.Should().BeFalse();
            }
            catch (AggregateException a)
            {
                a.InnerExceptions[0].Message.Should().Be("Intentionally failing");

                messages.Should().Contain("one");
                messages.Should().Contain("two");    
            }
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
