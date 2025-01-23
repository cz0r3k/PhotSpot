using Grpc.Core;
using GrpcPhoto;
using Microsoft.AspNetCore.Authorization;
using server_api.Services;

namespace server_api.ServicesGRPC;

[Authorize]
internal class PhotoServiceGrpc(IHttpContextAccessor httpContextAccessor, IPhotoService photoService) : GrpcPhoto.Photo.PhotoBase
{
    [AllowAnonymous]
    public override async Task<LikePhotoReply> LikePhotoInsecure(LikePhotoInsecureRequest request, ServerCallContext context)
    {
        var email = request.Email;
        var photoId = request.PhotoId;
        var eventId = request.EventId;
        var result = await photoService.LikePhoto(email, Guid.Parse(eventId.Value), Guid.Parse(photoId.Value));
        return new LikePhotoReply { Success = result };
    }

    [AllowAnonymous]
    public override async Task<PhotoDetailsReply> GetPhotoDetailsInsecure(PhotoDetailsInsecureRequest request, ServerCallContext context)
    {
        var email = request.Email;
        var photoId = request.PhotoId;
        var eventId = request.EventId;
        var result = await photoService.GetPhotoDetails(email, Guid.Parse(eventId.Value), Guid.Parse(photoId.Value));
        return result is null ? new PhotoDetailsReply { } : result.ToPhotoDetailsReply();
    }
}