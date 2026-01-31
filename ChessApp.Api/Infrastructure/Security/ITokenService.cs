using ChessApp.API.Models;

namespace ChessApp.API.Infrastructure.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
