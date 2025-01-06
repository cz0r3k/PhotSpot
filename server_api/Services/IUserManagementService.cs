using server_api.Identity;

namespace server_api.Services;

internal interface IUserManagementService
{
    public Task<AppUser?> RegisterNewUser(string email, string username);
    public Task<AppUser?> RegisterNewAdmin(string email, string username);
    public Task<bool> IsAdmin(string email);
    public Task<bool> IsRegistered(string email);
}