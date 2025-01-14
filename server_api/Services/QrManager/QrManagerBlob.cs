using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace server_api.Services.QrManager;

public class QrManagerBlob(BlobServiceClient blobServiceClient) : IQrManager
{
    private const string blobContainerName = "events";

    public async Task Save(Guid eventId, byte[] qrCode)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
        var blobClient = containerClient.GetBlobClient($"{eventId.ToString()}.png");
        await blobClient.UploadAsync(new MemoryStream(qrCode), new BlobHttpHeaders { ContentType = "image/png" });
    }
}