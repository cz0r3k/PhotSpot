using System.Security.Claims;
using Azure.Storage.Blobs;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcEvent;
using Microsoft.AspNetCore.Authorization;
using server_api.Data;
using server_api.Services;
using server_api.Services.PhotosManager;

namespace server_api.ServicesGRPC;

[Authorize]
internal class PhotoEventServiceGrpc(
    IHttpContextAccessor httpContextAccessor,
    IPhotoEventService photoEventService) : GrpcEvent.PhotoEvent.PhotoEventBase
{
    public override async Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        decimal latitude = 0;
        decimal longitude = 0;
        var parseResult = false;
        if (request.Location != null)
        {
            parseResult = decimal.TryParse(request.Location.Latitude, out latitude) && decimal.TryParse(request.Location.Longitude, out longitude);
        }
        var photoEventArgs = parseResult? new PhotoEventArgs
        {
            Name = request.Name, Latitude = latitude, Longitude = longitude,
        }: new PhotoEventArgs
        {
            Name = request.Name,
        };
        var eventId = await photoEventService.Create(email, photoEventArgs);
        return new CreateReply { Id = new UUID { Value = eventId?.ToString() } };
    }

    public override async Task<DetailsReply> GetDetails(UUID request, ServerCallContext context)
    {
        var photoEvent = await photoEventService.GetDetails(Guid.Parse(request.Value));
        return photoEvent == null ? new DetailsReply { } : photoEvent.ToPhotoEventDetails().ToDetailsReply();
    }

    [AllowAnonymous]
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

    [AllowAnonymous]
    public override async Task<PhotoReply> GetPhotos(UUID request, ServerCallContext context)
    {
        var photoIds = await photoEventService.GetPhotos(Guid.Parse(request.Value));

        var reply = new PhotoReply();
        foreach (var photoId in photoIds)
        {
            reply.PhotoIds.Add(new UUID { Value = photoId.ToString() });
        }

        return reply;
    }

    [AllowAnonymous]
    public override async Task<UploadStatus> AddPhoto(IAsyncStreamReader<PhotoChunk> requestStream,
        ServerCallContext context)
    {
        try
        {
            var headers = context.RequestHeaders;
            string eventIdHeader = headers.GetValue("eventId");
            string emailHeader = headers.GetValue("email");

            if (string.IsNullOrEmpty(eventIdHeader) || string.IsNullOrEmpty(emailHeader))
            {
                return new UploadStatus
                {
                    Success = false,
                    Message = "Event ID or Email is missing in the headers."
                };
            }

            Guid eventId = Guid.Parse(eventIdHeader);

            byte[] photo;
            using (var photoStream = new MemoryStream())
            {
                while (await requestStream.MoveNext())
                {
                    var chunk = requestStream.Current;
                    await photoStream.WriteAsync(chunk.Data.ToByteArray());
                }

                photo = photoStream.ToArray();
            }

            await photoEventService.AddPhoto(eventId, emailHeader, photo); // zwraca guid jakby ktos chcial
        }
        catch (Exception ex)
        {
            return new UploadStatus
            {
                Success = false,
                Message = $"Error uploading photo: {ex.Message}"
            };
        }

        return new UploadStatus
        {
            Success = true,
            Message = "Photo uploaded successfully"
        };
    }
}