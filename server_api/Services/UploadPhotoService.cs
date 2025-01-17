using Grpc.Core;
using GrpcPhotos;
using Microsoft.AspNetCore.Authorization;

namespace server_api.Services
{
    public class PhotosAService : PhotosA.PhotosABase
    {
        public override async Task<UploadStatus> UploadPhoto(IAsyncStreamReader<PhotoChunk> requestStream, ServerCallContext context)
        {
            try
            {
                string fileName = $"TempPhotos/zdj_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    while (await requestStream.MoveNext())
                    {
                        var chunk = requestStream.Current;
                        await fileStream.WriteAsync(chunk.Data.ToByteArray());
                    }
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
