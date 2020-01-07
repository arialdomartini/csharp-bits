using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace CSharpBits.Test.SampleGrpc
{
    public class GrcpTest<TStartup> : IDisposable
        where TStartup : class
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        protected GrpcChannel GetChannel() =>
            GrpcChannel.ForAddress(
                _client.BaseAddress,
                new GrpcChannelOptions
                {
                    HttpClient = _client
                });

        protected GrcpTest()
        {
            var builder = new HostBuilder()
                .ConfigureServices(services => { })
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost
                        .UseTestServer()
                        .UseStartup<TStartup>();
                });
            var host = builder.Start();

            _server = host.GetTestServer();

            var responseVersionHandler = new ResponseVersionHandler {InnerHandler = _server.CreateHandler()};

            _client = new HttpClient(responseVersionHandler) { BaseAddress = new Uri("http://localhost")};
        }

        /// <summary>
        /// Needed to set the response version to 2.0.
        /// This is required because of this TestServer issue: https://github.com/aspnet/AspNetCore/issues/16940
        /// </summary>
        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;

                return response;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}