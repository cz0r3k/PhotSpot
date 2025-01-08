namespace server_api.Services.PhotosManager;

public interface IPhotoManager
{
    public Task Save(Guid photoId, Guid eventId, byte[] photo);
}