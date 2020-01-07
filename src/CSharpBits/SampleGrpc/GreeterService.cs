using System.Threading.Tasks;
using Grpc.Core;

namespace CSharpBits.SampleGrpc
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) =>
            Task.FromResult(new HelloReply {Message = $"Hi, {request.Name}!"});
    }
}