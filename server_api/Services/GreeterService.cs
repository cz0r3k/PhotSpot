using Grpc.Core;
using GrpcGreeter;
using Microsoft.AspNetCore.Authorization;

namespace server_api.Services
{
    [Authorize]
    public class GreeterService(ILogger<GreeterService> logger, IHttpContextAccessor httpContextAccessor)
        : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var user = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {user?.Value} aka {request.Name}"
            });
        }
    }
}