using System.Security.Cryptography;
using System.Text;

namespace ChessApp.API.Infrastructure.Security;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string Hash(string token)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }
}
