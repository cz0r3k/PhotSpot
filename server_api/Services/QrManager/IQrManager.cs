namespace server_api.Services.QrManager;

public interface IQrManager
{
    public Task Save(Guid eventId,byte[] qrCode);
}