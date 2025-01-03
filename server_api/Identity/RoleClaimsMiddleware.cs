using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace server_api.Identity;

internal class RoleClaimsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated ?? false)
        {
            var appUser = await userManager.FindByEmailAsync(context.User.FindFirstValue(ClaimTypes.Email)!);
            if (appUser != null)
            {
                var roles = await userManager.GetRolesAsync(appUser);
                foreach (var role in roles)
                {
                    ((ClaimsIdentity)context.User.Identity!).AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }
        await next(context);
    }
}