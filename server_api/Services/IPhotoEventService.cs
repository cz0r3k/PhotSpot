using server_api.Data;

namespace server_api.Services;

internal interface IPhotoEventService
{
    public Task<bool> Create(string email, PhotoEventArgs photoEventArgs);
}