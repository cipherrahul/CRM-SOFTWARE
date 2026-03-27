using CRM.Shared.Application;
using CRM.Shared.Common;
using IdentityService.Domain.Repositories;

namespace IdentityService.Application.Queries.GetUser;

internal sealed class GetUserByIdQueryHandler
    : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
        => _userRepository = userRepository;

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return Error.NotFound with { Code = "User.NotFound", Description = $"User {request.UserId} was not found." };

        return new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            user.IsActive,
            user.CreatedAt,
            user.LastLoginAt);
    }
}
