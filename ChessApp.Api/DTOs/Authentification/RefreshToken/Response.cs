namespace ChessApp.API.DTOs.Authentification;

public sealed class RefreshTokenResponse
{
    public string Token { get; init; } = default!;
    public string? RefreshToken { get; init; }
}