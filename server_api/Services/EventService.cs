using System.Security.Claims;
using Grpc.Core;
using GrpcEvent;
using Microsoft.AspNetCore.Authorization;
using QRCoder;
using server_api.Data;
using Event = GrpcEvent.Event;

namespace server_api.Services;

[Authorize]
internal class EventService(
    ILogger<UserManagementService> logger,
    IHttpContextAccessor httpContextAccessor,
    AppDbContext appDbContext) : Event.EventBase
{
    public override async Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var user = await appDbContext.Users.FindAsync(email);
        if (user == null)
        {
            logger.LogInformation($"User {email} is not registered.");
            return new CreateReply { Message = false };
        }
        var eventA = new Data.Event
        {
            Owner = user,
            Name = request.Name,
            ExpirationDate = DateTime.Today.AddDays(1),
            MinimalTimespan = TimeSpan.FromHours(6),
        };
        await appDbContext.Events.AddAsync(eventA);
        var qrGenerator = new QRCodeGenerator();
        var eventPayload = new EventPayload { EventId = eventA.Id, ExpirationDate = eventA.ExpirationDate };
        var qrCodeData = qrGenerator.CreateQrCode(eventPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        await File.WriteAllBytesAsync($"events/{eventA.Id}.png", qrCode.GetGraphic(20));
        return new CreateReply{ Message =  true };
    }
}