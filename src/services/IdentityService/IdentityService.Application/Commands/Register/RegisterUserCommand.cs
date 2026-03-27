using CRM.Shared.Application;
using CRM.Shared.Common;

namespace IdentityService.Application.Commands.Register;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "User") : ICommand<Result<RegisterUserResponse>>;

public sealed record RegisterUserResponse(
    Guid   UserId,
    string Email,
    string FullName,
    string Role);
