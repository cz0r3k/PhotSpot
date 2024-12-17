namespace web_admin
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken);
    }
}