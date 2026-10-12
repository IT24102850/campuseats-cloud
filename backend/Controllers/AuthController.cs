using CampusEats.Api.Data;
using CampusEats.Api.Dtos;
using CampusEats.Api.Models;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CampusEats.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // -> /api/auth
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;

    public AuthController(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")] // POST /api/auth/register
    public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { message = "Email already registered" });

        var user = new AppUser { Email = email, PasswordHash = Hash(dto.Password) };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokens.Create(user);
        return Ok(new AuthResultDto(user.Email, token.Value, token.ExpiresAt));
    }

    [HttpPost("login")] // POST /api/auth/login
    public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Same response for "no such user" and "wrong password" so the
        // endpoint does not reveal which accounts exist.
        if (user is null || !Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password" });

        var token = _tokens.Create(user);
        return Ok(new AuthResultDto(user.Email, token.Value, token.ExpiresAt));
    }

    // PBKDF2 with a per-user random salt, stored as salt.hash
    private static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(password, salt,
            KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = KeyDerivation.Pbkdf2(password, salt,
            KeyDerivationPrf.HMACSHA256, 100_000, 32);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
