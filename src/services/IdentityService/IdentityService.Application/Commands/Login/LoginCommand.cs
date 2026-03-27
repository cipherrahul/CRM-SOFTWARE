using CRM.Shared.Application;
using CRM.Shared.Common;

namespace IdentityService.Application.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<Result<LoginResponse>>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Role,
    string FullName,
    Guid UserId);
