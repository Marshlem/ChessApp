namespace ChessApp.API.DTOs.Authentification;

public sealed class RegisterRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}