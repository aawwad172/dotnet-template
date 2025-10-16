

using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class RegisterUserCommandHandler(
    ICurrentUserService currentUserService,
    ILogger<RegisterUserCommandHandler> logger,
    IAuthenticationRepository authenticationRepository,
    IUserRepository userRepository,
    ISecurityService securityService,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : BaseHandler<RegisterUserCommand, RegisterUserCommandResult>(currentUserService, logger, unitOfWork)
{
    private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISecurityService _securityService = securityService;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly string _defaultRoleName = "User";

    public async override Task<RegisterUserCommandResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if a user already exists with the same email.
        User? existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new ConflictException("A user with this email already exists.");

        // Check if a user already exists with the same username.
        User? existingUsername = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (existingUsername is not null)
            throw new ConflictException("A user with this username already exists.");

        // 1. Hash the password (returns combined HASH-SALT string)
        string hashedPassword = _securityService.HashSecret(request.Password);

        // 2. Split the combined string into its two secure parts
        string securityStamp = Guid.NewGuid().ToString();

        Guid id = Guid.CreateVersion7();

        Role? defaultRole = await _roleRepository.GetRoleByNameAsync(_defaultRoleName);

        if (defaultRole is null)
        {
            // IMPORTANT: This prevents users from being created without a role if the DB isn't seeded.
            throw new InvalidOperationException($"The default role '{_defaultRoleName}' does not exist in the database. Please seed roles.");
        }

        User user = new()
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty,
            IsActive = false,
            IsDeleted = false,
            IsVerified = false,
            SecurityStamp = securityStamp,
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _ = _userRepository.AddAsync(user);

            UserRole userRole = new()
            {
                UserId = user.Id,
                RoleId = defaultRole.Id,
            };
            await _authenticationRepository.AddUserRoleAsync(userRole); // Assuming IAuthRepository has this method

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            return new RegisterUserCommandResult(user, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }

    }
}
