namespace server_api.Services;

public interface IPhotoService
{
    public Task<bool> LikePhoto(string email, Guid eventId, Guid photoId);
}