using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcUser;
using Microsoft.AspNetCore.Authorization;
using server_api.Services;
using RegisterRequest = GrpcUser.RegisterRequest;

namespace server_api.ServicesGRPC;

[Authorize]
internal class UserManagementServiceGrpc(
    IHttpContextAccessor httpContextAccessor,
    IUserManagementService userManagementService) : UserManagement.UserManagementBase
{
    public override async Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var username = request.Name;
        var user = await userManagementService.RegisterNewUser(email, username);
        return new RegisterReply { Message = user is not null };
    }
    
    public override async Task<RegisterReply> RegisterAdmin(RegisterRequest request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        var username = request.Name;
        var user = await userManagementService.RegisterNewAdmin(email, username);
        return new RegisterReply { Message = user is not null };
    }

    public override async Task<IsRegisterReply> IsRegistered(Empty request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        return new IsRegisterReply{Message = await userManagementService.IsRegistered(email)};
    }

    public override async Task<IsAdminReply> IsAdmin(Empty request, ServerCallContext context)
    {
        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        return new IsAdminReply{Message = await userManagementService.IsAdmin(email)};
    }

    [AllowAnonymous]
    public override async Task<RegisterReply> RegisterInsecure(RegisterInsecureRequest request, ServerCallContext context)
    {
        var email = request.Email;
        var username = request.Name;
        var user = await userManagementService.RegisterNewAdmin(email, username);
        return new RegisterReply { Message = user is not null };
    }

    [AllowAnonymous]
    public override async Task<IsRegisterReply> IsRegisteredInsecure(IsRegisteredInsecureRequest request, ServerCallContext context)
    {
        var email = request.Email;
        return new IsRegisterReply{Message = await userManagementService.IsRegistered(email)};
    }
}