using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces;

/// <summary>Generates and validates JWT tokens.</summary>
public interface IJwtService
{
    (string Token, DateTime Expiry) GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid?  ValidateAccessToken(string token);
}
