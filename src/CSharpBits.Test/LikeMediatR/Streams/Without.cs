using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.Streams
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
            builder.RegisterType<PlainPingStreamHandler>().As<IStreamPingHandler>().InstancePerLifetimeScope();
            return builder;
        }
        
        [Fact]
        async void use_ping_stream()
        {
            var handler = _scope.Resolve<IStreamPingHandler>();

            var stream = handler.Ping(new StreamPing{Message = "Hey!"});

            var i = 1;
            await foreach (var value in stream)
            {
                Assert.Equal($"Hey! Pong {i}", value.Message);
                i++;
            }
        }
    }

    internal class PlainPingStreamHandler : IStreamPingHandler
    {
        async IAsyncEnumerable<Pong> IStreamPingHandler.Ping(StreamPing ping)
        {
            yield return await Task.Run(() => new Pong { Message = $"{ping.Message} Pong 1"});
            yield return await Task.Run(() => new Pong { Message = $"{ping.Message} Pong 2"});
            yield return await Task.Run(() => new Pong { Message = $"{ping.Message} Pong 3"});
        }
    }

    internal interface IStreamPingHandler
    {
        IAsyncEnumerable<Pong> Ping(StreamPing ping);
    }
}
