namespace server_api.Services.QrManager;

public class QrManagerFile : IQrManager
{
    public async Task Save(Guid eventId, byte[] qrCode)
    {
        await File.WriteAllBytesAsync($"events/{eventId}.png", qrCode);
    }
}