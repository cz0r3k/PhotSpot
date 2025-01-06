using System.Security.Claims;
using Grpc.Core;
using GrpcEvent;
using Microsoft.AspNetCore.Authorization;
using server_api.Data;
using server_api.Services;
using Event = GrpcEvent.Event;

namespace server_api.ServicesGRPC;

[Authorize]
internal class PhotoEventServiceGrpc(
    IHttpContextAccessor httpContextAccessor,
    IPhotoEventService photoEventService) : Event.EventBase
{
    public override async Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var photoEventArgs = new PhotoEventArgs { Name = request.Name };
        return new CreateReply{ Message =  await photoEventService.Create(email, photoEventArgs)};
    }
}