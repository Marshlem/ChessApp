using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChessApp.API.Infrastructure;

public static class UserContext
{
    public static int GetUserId(ClaimsPrincipal user)
        => int.Parse(
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("Missing user id claim"));
}
