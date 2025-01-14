using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace server_api.Services.PhotosManager;

public class PhotoManagerBlob(BlobServiceClient blobServiceClient) : IPhotoManager
{
    private const string blobContainerName = "photos";

    public async Task Save(Guid photoId, Guid eventId, byte[] photo)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
        var blobClient = containerClient.GetBlobClient($"{eventId.ToString()}/{photoId.ToString()}.png");
        await blobClient.UploadAsync(new MemoryStream(photo), new BlobHttpHeaders { ContentType = "image/png" });
    }
}