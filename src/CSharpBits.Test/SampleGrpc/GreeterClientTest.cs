using System.Threading.Tasks;
using CSharpBits.SampleGrpc;
using FluentAssertions;
using Xunit;

namespace CSharpBits.Test.SampleGrpc
{
    public class GreeterClientTest : GrcpTest<Startup>
    {
        [Fact]
        async Task invokes_Greeter_service_using_gRPC()
        {
            using (var channel = GetChannel())
            {
                var client = new Greeter.GreeterClient(channel);

                var reply = await client.SayHelloAsync(new HelloRequest {Name = "Mario"});

                reply.Message.Should().Be("Hi, Mario!");
            }
        }
    }
}