using System.Security.Claims;
using Grpc.Core;
using GrpcUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RegisterRequest = GrpcUser.RegisterRequest;
using Google.Protobuf.WellKnownTypes;

namespace server_api.Services;

[Authorize]
internal class UserManagementService(
    ILogger<UserManagementService> logger,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager) : UserManagement.UserManagementBase
{
    public override async Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            logger.LogInformation($"User {email} is already registered.");
            return new RegisterReply { Message = false };
        }
        user = new AppUser()
        {
            UserName = request.Name,
            Email = email,
        };
        await userManager.CreateAsync(user);
        logger.LogInformation($"User {email} is now registered.");
        return new RegisterReply { Message = true };
    }
    
    public override async Task<RegisterReply> RegisterAdmin(RegisterRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            logger.LogInformation($"User {email} is already registered.");
            return new RegisterReply { Message = false };
        }
        user = new AppUser()
        {
            UserName = request.Name,
            Email = email,
        };
        await userManager.CreateAsync(user);
        await userManager.AddToRoleAsync(user, "Admin");
        logger.LogInformation($"User {email} is now registered as administrator.");
        return new RegisterReply { Message = true };
    }

    public override async Task<IsRegisterReply> IsRegistered(Empty request, ServerCallContext context)
    {
        return new IsRegisterReply{Message = await GetUser(httpContextAccessor.HttpContext!.User) != null};
    }

    public override async Task<IsAdminReply> IsAdmin(Empty request, ServerCallContext context)
    {
        var user = await GetUser(httpContextAccessor.HttpContext!.User);
        return new IsAdminReply{Message = user != null && (await userManager.GetRolesAsync(user)).Contains("Admin")};
    }

    private async Task<AppUser?> GetUser(ClaimsPrincipal principal)
    {
        var email = principal.FindFirstValue(ClaimTypes.Email)!;
        return await userManager.FindByEmailAsync(email);
    }
}