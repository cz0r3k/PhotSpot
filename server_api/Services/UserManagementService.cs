using Microsoft.AspNetCore.Identity;
using server_api.Data;
using server_api.Identity;

namespace server_api.Services;

internal class UserManagementService(
    ILogger<UserManagementService> logger,
    UserManager<AppUser> userManager,
    AppDbContext appDbContext) : IUserManagementService
{
    public async Task<AppUser?> RegisterNewUser(string email, string username)
    {
        // TODO add email and username validation
        var user = await userManager.FindByEmailAsync(email);
        if (user != null) return null;
        user = new AppUser { Email = email, UserName = username };
        await userManager.CreateAsync(user);
        await appDbContext.Users.AddAsync(new User {Email = email, Username = username});
        await appDbContext.SaveChangesAsync();
        logger.LogInformation($"User {email} registered");
        return user;
    }

    public async Task<AppUser?> RegisterNewAdmin(string email, string username)
    {
        var user = await RegisterNewUser(email, username);
        if (user == null) return null;
        await userManager.AddToRoleAsync(user, "Admin");
        logger.LogInformation($"User {email} registered as admin");
        return user;
    }

    public async Task<bool> IsAdmin(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user != null && (await userManager.IsInRoleAsync(user, "Admin"));
    }

    public async Task<bool> IsRegistered(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user != null;
    }
}