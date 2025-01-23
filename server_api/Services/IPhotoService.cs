using util.Photo;

namespace server_api.Services;

public interface IPhotoService
{
    public Task<bool> LikePhoto(string email, Guid eventId, Guid photoId);
    public Task<bool> UnLikePhoto(string email, Guid eventId, Guid photoId);
    public Task<PhotoDetails?> GetPhotoDetails(string email, Guid eventId, Guid photoId);
}