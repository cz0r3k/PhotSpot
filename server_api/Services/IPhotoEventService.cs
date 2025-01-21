using server_api.Data;
using util.PhotoEvent;
using PhotoEvent = server_api.Data.PhotoEvent;

namespace server_api.Services;

internal interface IPhotoEventService
{
    public Task<Guid?> Create(string email, PhotoEventArgs photoEventArgs);
    public Task<PhotoEvent?> GetDetails(Guid id);
    public Task<IEnumerable<PhotoEventSimple>> GetActiveEvents();
    public Task<Guid?> AddPhoto(Guid eventId, string email, byte[] photoData);
    public Task<IEnumerable<Guid>> GetPhotos(Guid eventId);

}