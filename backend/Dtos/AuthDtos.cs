using System.ComponentModel.DataAnnotations;

namespace CampusEats.Api.Dtos;

public record RegisterDto(
    [Required, EmailAddress, StringLength(120)] string Email,
    [Required, StringLength(100, MinimumLength = 6)] string Password);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

// What the client receives after a successful login.
public record AuthResultDto(string Email, string Token, DateTime ExpiresAt);
