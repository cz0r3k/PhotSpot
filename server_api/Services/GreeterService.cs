using Grpc.Core;
using GrpcGreeter;
using Microsoft.AspNetCore.Authorization;
using QRCoder;
using server_api.Data;

namespace server_api.Services
{
    [Authorize]
    public class GreeterService(ILogger<GreeterService> logger, IHttpContextAccessor httpContextAccessor)
        : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var user = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            EventPayload eventPayload = new EventPayload() {EventId = Guid.CreateVersion7(), ExpirationDate = DateTime.UtcNow.AddHours(1)};
            
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(eventPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            File.WriteAllBytes("qrcode.png", qrCodeAsPngByteArr);
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {user?.Value} aka {request.Name}"
            });
        }
    }
}