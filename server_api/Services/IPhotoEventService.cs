using server_api.Data;

namespace server_api.Services;

internal interface IPhotoEventService
{
    public Task<Guid?> Create(string email, PhotoEventArgs photoEventArgs);
    public Task<PhotoEvent?> GetDetails(Guid id);
}