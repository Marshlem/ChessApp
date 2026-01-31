using ChessApp.API.Data;
using ChessApp.API.Models;
using ChessApp.API.DTOs.Authentification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChessApp.API.Infrastructure.Security;
using Microsoft.AspNetCore.RateLimiting;

namespace ChessApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        ApplicationDbContext db,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _db = db;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return Unauthorized("Invalid credentials");

        if (user.LockoutUntilUtc > DateTime.UtcNow)
            return Unauthorized("Invalid credentials");

        if (user.Provider != AuthProvider.Local || user.PasswordHash == null)
            return Unauthorized("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= 5)
            {
                user.LockoutUntilUtc = DateTime.UtcNow.AddMinutes(10);
                user.FailedLoginCount = 0;
            }

            await _db.SaveChangesAsync();
            return Unauthorized("Invalid credentials");
        }

        user.FailedLoginCount = 0;
        user.LockoutUntilUtc = null;
        await _db.SaveChangesAsync();

        var token = _tokenService.GenerateAccessToken(user);
        return Ok(new LoginResponse { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Provider = AuthProvider.Local,
            EmailVerified = false
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(); 

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Unauthorized();

        var tokenHash = _refreshTokenService.Hash(request.RefreshToken);

        var storedToken = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash &&
                !x.Revoked &&
                x.ExpiresAtUtc > DateTime.UtcNow);

        if (storedToken == null)
            return Unauthorized();

        storedToken.Revoked = true;

        var newRefreshToken = _refreshTokenService.Generate();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = _refreshTokenService.Hash(newRefreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
        });

        await _db.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Token = _tokenService.GenerateAccessToken(storedToken.User),
            RefreshToken = newRefreshToken
        });
    }
}
