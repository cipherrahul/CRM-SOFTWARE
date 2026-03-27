using CRM.Shared.Application;
using CRM.Shared.Common;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Repositories;

namespace IdentityService.Application.Commands.Login;

internal sealed class LoginCommandHandler
    : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher  _passwordHasher;
    private readonly IJwtService      _jwtService;
    private readonly IUnitOfWork      _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService     = jwtService;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive)
            return Error.Unauthorized with { Code = "Auth.InvalidCredentials", Description = "Invalid email or password." };

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Error.Unauthorized with { Code = "Auth.InvalidCredentials", Description = "Invalid email or password." };

        var (accessToken, expiry) = _jwtService.GenerateAccessToken(user);
        var refreshToken          = _jwtService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordLogin();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken, expiry, user.Role.ToString(), user.FullName, user.Id);
    }
}
