using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.Streams
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
                .RegisterAssemblyTypes(typeof(PingHandler).GetTypeInfo().Assembly).AsImplementedInterfaces();
            //builder.RegisterType<PingHandler>().AsImplementedInterfaces().InstancePerDependency();          // or individually

            return builder;
        }

        [Fact]
        async void use_ping_stream()
        {
            var mediator = _scope.Resolve<IMediator>();

            IAsyncEnumerable<Pong> stream = mediator.CreateStream(new StreamPing{Message = "Hey!"});

            var i = 1;
            await foreach (var value in stream)
            {
                Assert.Equal($"Hey! Pong {i}", value.Message);
                i++;
            }
        }
    }

    class PingHandler :
        IStreamRequestHandler<StreamPing, Pong>
    {
        async IAsyncEnumerable<Pong> IStreamRequestHandler<StreamPing, Pong>.Handle(StreamPing ping, CancellationToken cancellationToken)
        {
            yield return await Task.Run(() => new Pong { Message = $"{ping.Message} Pong 1"}, cancellationToken);
            yield return await Task.Run(() => new Pong { Message = $"{ping.Message} Pong 2"}, cancellationToken);
            yield return await Task.Run(() => new Pong { Message = $"{ping.Message} Pong 3"}, cancellationToken);
        }
    }

    public class StreamPing : IStreamRequest<Pong>
    {
        public string Message { get; set; }
    }

    public class Pong 
    {
        public string Message { get; set; }
        
    }
}
