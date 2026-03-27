namespace IdentityService.Application.Interfaces;

/// <summary>Bcrypt-based password hashing abstraction.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool   Verify(string password, string hash);
}
