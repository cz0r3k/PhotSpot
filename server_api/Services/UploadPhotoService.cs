using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Grpc.Core;
using GrpcPhotos;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using server_api.Services.PhotosManager;

namespace server_api.Services
{
    public class PhotosAService : PhotosA.PhotosABase
    {
        private readonly BlobServiceClient _blobServiceClient;
        public PhotosAService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public override async Task<UploadStatus> UploadPhoto(IAsyncStreamReader<PhotoChunk> requestStream, ServerCallContext context)
        {
            try
            {
                using (var photoStream = new MemoryStream())
                {
                    while (await requestStream.MoveNext())
                    {
                        var chunk = requestStream.Current;
                        await photoStream.WriteAsync(chunk.Data.ToByteArray());
                    }
                    byte[] photo = photoStream.ToArray();

                    Guid photo_guid = Guid.NewGuid();
                    Guid eventId_guid = Guid.NewGuid(); // TODO zmienic na zaciagane ze streama!!

                    var photosManager = new PhotoManagerBlob(_blobServiceClient);
                    await photosManager.Save(photo_guid, eventId_guid, photo);
                }
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

}
