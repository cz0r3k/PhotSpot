using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcEvent;
using Microsoft.AspNetCore.Authorization;
using server_api.Data;
using server_api.Services;

namespace server_api.ServicesGRPC;

[Authorize]
internal class PhotoEventServiceGrpc(
    IHttpContextAccessor httpContextAccessor,
    IPhotoEventService photoEventService) : GrpcEvent.PhotoEvent.PhotoEventBase
{
    public override async Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var photoEventArgs = new PhotoEventArgs { Name = request.Name };
        var eventId = await photoEventService.Create(email, photoEventArgs);
        return new CreateReply{ Id = new UUID{Value = eventId?.ToString()}};
    }

    public override async Task<DetailsReply> GetDetails(UUID request, ServerCallContext context)
    {
        var photoEvent = await photoEventService.GetDetails(Guid.Parse(request.Value));
        return photoEvent == null ? new DetailsReply {  } : photoEvent.ToPhotoEventDetails().ToDetailsReply();
    }

    public override async Task<SimpleReply> GetActiveEvents(Empty request, ServerCallContext context)
    {
        var photoEvents = await photoEventService.GetActiveEvents();
        var reply = new SimpleReply();
        foreach (var photoEvent in photoEvents)
        {
            reply.Event.Add(photoEvent.ToEventSimple());
        }
        return reply;
    }
}