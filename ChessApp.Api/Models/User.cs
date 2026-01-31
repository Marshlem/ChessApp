namespace ChessApp.API.Models;
public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = default!;
    public bool EmailVerified { get; set; }

    public string? PasswordHash { get; set; }

    public string? GoogleSubject { get; set; } 
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutUntilUtc { get; set; }  
    public AuthProvider Provider { get; set; } = AuthProvider.Local;
}
