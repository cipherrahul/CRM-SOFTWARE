using CRM.Shared.Application;
using CRM.Shared.Common;

namespace IdentityService.Application.Queries.GetUser;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserDto>>;

public sealed record UserDto(
    Guid   Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool   IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);
