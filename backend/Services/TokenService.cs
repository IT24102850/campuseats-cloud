using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CampusEats.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace CampusEats.Api.Services;

// Signs JWTs using the key/issuer/audience that come from
// configuration. In production those values arrive as the
// environment variables Jwt__Key, Jwt__Issuer and Jwt__Audience,
// so no secret is ever compiled into the image.
public class TokenService
{
    private readonly IConfiguration _cfg;

    public TokenService(IConfiguration cfg) => _cfg = cfg;

    public AuthToken Create(AppUser user)
    {
        var key = _cfg["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured");
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(2);
        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            },
            expires: expires,
            signingCredentials: creds);

        return new AuthToken(
            new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}

public record AuthToken(string Value, DateTime ExpiresAt);
