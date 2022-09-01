using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Xunit;

namespace CSharpBits.Test.LikeMediatR.Ping
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

        [Fact]
        async void use_ping()
        {
            var mediator = _scope.Resolve<IMediator>();

            var result = await mediator.Send(new Ping());
            
            Assert.Equal("Pong", result);
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
    }
    
    // Cannot be internal; can be nested
    public class Ping : IRequest<string>{}

    // Can be internal
    internal class PingHandler : IRequestHandler<Ping, string>
    {
        Task<string> IRequestHandler<Ping, string>.Handle(Ping request, CancellationToken cancellationToken) =>
            Task.FromResult("Pong");
    }
}
