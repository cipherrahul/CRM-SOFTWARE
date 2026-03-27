using CRM.Shared.Domain;
using IdentityService.Domain.Events;
using IdentityService.Domain.ValueObjects;

namespace IdentityService.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    public string FirstName { get; private set; } = default!;
    public string LastName  { get; private set; } = default!;
    public Email  Email     { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public Role   Role      { get; private set; }
    public bool   IsActive  { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    private User() { } // EF Core

    public static User Create(string firstName, string lastName, string email, string passwordHash, Role role = Role.User)
    {
        var user = new User
        {
            Id           = Guid.NewGuid(),
            FirstName    = firstName,
            LastName     = lastName,
            Email        = Email.Create(email),
            PasswordHash = passwordHash,
            Role         = role,
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value, user.FirstName));
        return user;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void SetRefreshToken(string token, DateTime expiry)
    {
        RefreshToken       = token;
        RefreshTokenExpiry = expiry;
    }

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void Deactivate() => IsActive = false;

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName  = lastName;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash   = newPasswordHash;
        RefreshToken   = null;
        RefreshTokenExpiry = null;
    }
}
