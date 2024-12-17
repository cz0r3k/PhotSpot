using Microsoft.AspNetCore.Authentication;

namespace web_admin;

public class AppTokenProvider(IHttpContextAccessor httpContextAccessor) : ITokenProvider
{
    private string? _token;

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        _token ??= await httpContextAccessor.HttpContext!.GetTokenAsync("id_token");
        return _token!;
    }
}