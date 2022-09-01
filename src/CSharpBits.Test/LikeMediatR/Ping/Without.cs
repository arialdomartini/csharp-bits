using System;
using System.Threading.Tasks;
using Autofac;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.With
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

        private ContainerBuilder ConfigureAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PlainPingHandler>().As<IPingHandler>().InstancePerLifetimeScope();
            return builder;
        }

        [Fact]
        async void use_ping()
        {
            var handler = _scope.Resolve<IPingHandler>();
            
            var result = await handler.Ping();
            
            Assert.Equal("Pong", result);
        }
        
        [Fact]
        async void use_handle()
        {
            var handler = _scope.Resolve<IPingHandler>();
            
            var result = await handler.Handle(new PingRequest());
            
            Assert.Equal("Pong", result);
        }
        
        void IDisposable.Dispose()
        {
            _scope.Dispose();
            _container.Dispose();
        }
    }

    internal class PlainPingHandler : IPingHandler 
    {
        Task<string> IPingHandler.Ping() =>
            Task.FromResult("Pong");

        Task<string> IPingHandler.Handle(PingRequest ping) =>
            Task.FromResult("Pong");
    }

    internal interface IPingHandler
    {
        Task<string> Ping();
        Task<string> Handle(PingRequest ping);
    }

    internal class PingRequest { }
}
