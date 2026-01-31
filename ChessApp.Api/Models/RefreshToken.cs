namespace ChessApp.API.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string TokenHash { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public bool Revoked { get; set; }

    public User User { get; set; } = default!;
}
