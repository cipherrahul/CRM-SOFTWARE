using CRM.Shared.Application;
using CRM.Shared.Common;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Commands.Register;

internal sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher  _passwordHasher;
    private readonly IUnitOfWork      _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork     = unitOfWork;
    }

    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Check duplicate
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            return Error.Conflict with { Code = "User.EmailConflict", Description = $"Email '{request.Email}' is already registered." };

        // 2. Parse role
        if (!Enum.TryParse<Role>(request.Role, true, out var role))
            role = Role.User;

        // 3. Hash & create
        var hash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.FirstName, request.LastName, request.Email, hash, role);

        // 4. Persist
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserResponse(user.Id, user.Email.Value, user.FullName, user.Role.ToString());
    }
}
