using Grpc.Core;
using GrpcGreeter;
using Microsoft.AspNetCore.Authorization;
using QRCoder;
using server_api.Data;

namespace server_api.ServicesGRPC
{
    [Authorize]
    public class GreeterServiceGrpc(ILogger<GreeterServiceGrpc> logger, IHttpContextAccessor httpContextAccessor)
        : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var user = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            var photoEventPayload = new PhotoEventPayload()
                { EventId = Guid.CreateVersion7(), Name = "test", ExpirationDate = DateTime.UtcNow.AddHours(1) };
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(photoEventPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            File.WriteAllBytes("qrcode.png", qrCodeAsPngByteArr);
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {user?.Value} aka {request.Name}"
            });
        }
    }
}