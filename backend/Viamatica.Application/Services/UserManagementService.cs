using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Users;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class UserManagementService : IUserManagementService
{
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly IActiveUserReportQuery _activeUserReportQuery;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public UserManagementService(
        IUserManagementRepository userManagementRepository,
        IActiveUserReportQuery activeUserReportQuery,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userManagementRepository = userManagementRepository;
        _activeUserReportQuery = activeUserReportQuery;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(bool pendingOnly, CancellationToken cancellationToken = default)
        => _userManagementRepository.GetAllAsync(pendingOnly, cancellationToken);

    public async Task<UserResponseDto> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManagementRepository.GetByIdAsync(userId, cancellationToken);
        return user ?? throw new NotFoundException($"No se encontró el usuario {userId}.");
    }

    public Task<IReadOnlyCollection<ActiveUserReportDto>> GetActiveReportAsync(CancellationToken cancellationToken = default)
        => _activeUserReportQuery.GetAsync(cancellationToken);

    public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto request, int creatorUserId, string creatorRole, CancellationToken cancellationToken = default)
    {
        ValidateCreatorRole(creatorRole);

        if (request.RoleId == RoleIds.Administrator)
        {
            throw new ForbiddenOperationException("No se permite crear administradores desde la API.");
        }

        await EnsureUniqueAsync(request.UserName, request.Email, request.Identification, null, cancellationToken);
        await EnsureRoleExistsAsync(request.RoleId, cancellationToken);
        await EnsureActiveApprovedUserAsync(creatorUserId, cancellationToken);

        var hashedPassword = _passwordHasher.Hash(request.Password);
        var user = new User(request.UserName.Trim(), request.Identification.Trim(), request.Email.Trim(), hashedPassword, request.RoleId, UserStatusIds.Pending);

        if (creatorRole == RoleNames.Administrator)
        {
            user.Approve();
        }
        else
        {
            user.MarkPendingApproval();
        }

        _userManagementRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(user.UserId, cancellationToken);
    }

    public async Task<UserResponseDto> UpdateAsync(int userId, UpdateUserRequestDto request, string actorRole, CancellationToken cancellationToken = default)
    {
        ValidateCreatorRole(actorRole);

        var user = await _userManagementRepository.GetForUpdateAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario {userId}.");

        if (user.RoleId == RoleIds.Administrator && actorRole != RoleNames.Administrator)
        {
            throw new ForbiddenOperationException("Solo un administrador puede modificar otro administrador.");
        }

        if (request.RoleId == RoleIds.Administrator)
        {
            throw new ForbiddenOperationException("No se permite asignar el rol administrador.");
        }

        await EnsureUniqueAsync(request.UserName, request.Email, request.Identification, userId, cancellationToken);
        await EnsureRoleExistsAsync(request.RoleId, cancellationToken);

        user.UpdateProfile(request.UserName.Trim(), request.Identification.Trim(), request.Email.Trim(), request.RoleId);

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.ChangePassword(_passwordHasher.Hash(request.Password.Trim()));
        }

        if (!request.Active)
        {
            user.Deactivate();
        }
        else if (user.UserApproval == 1)
        {
            user.Activate();
        }
        else
        {
            user.MarkPendingApproval();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(user.UserId, cancellationToken);
    }

    public async Task ApproveAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManagementRepository.GetForUpdateAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario {userId}.");

        user.Approve();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int userId, string actorRole, CancellationToken cancellationToken = default)
    {
        ValidateCreatorRole(actorRole);

        var user = await _userManagementRepository.GetForUpdateAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario {userId}.");

        if (user.RoleId == RoleIds.Administrator && actorRole != RoleNames.Administrator)
        {
            throw new ForbiddenOperationException("Solo un administrador puede eliminar otro administrador.");
        }

        _userManagementRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueAsync(string userName, string email, string identification, int? currentUserId, CancellationToken cancellationToken)
    {
        var normalizedUserName = userName.Trim();
        var normalizedEmail = email.Trim();
        var normalizedIdentification = identification.Trim();

        var userNameExists = await _userManagementRepository.UserNameExistsAsync(normalizedUserName, currentUserId, cancellationToken);

        if (userNameExists)
        {
            throw new ConflictException("El username ya existe.");
        }

        var emailExists = await _userManagementRepository.EmailExistsAsync(normalizedEmail, currentUserId, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("El email ya existe.");
        }

        var identificationExists = await _userManagementRepository.IdentificationExistsAsync(normalizedIdentification, currentUserId, cancellationToken);

        if (identificationExists)
        {
            throw new ConflictException("La identificación del usuario ya existe.");
        }
    }

    private async Task EnsureRoleExistsAsync(int roleId, CancellationToken cancellationToken)
    {
        var exists = await _userManagementRepository.RoleExistsAsync(roleId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"No existe el rol {roleId}.");
        }
    }

    private async Task EnsureActiveApprovedUserAsync(int userId, CancellationToken cancellationToken)
    {
        var exists = await _userManagementRepository.IsActiveApprovedUserAsync(userId, cancellationToken);

        if (!exists)
        {
            throw new ForbiddenOperationException("El usuario autenticado no está activo o aprobado.");
        }
    }

    private static void ValidateCreatorRole(string creatorRole)
    {
        if (creatorRole is not RoleNames.Administrator and not RoleNames.Gestor)
        {
            throw new ForbiddenOperationException("No tiene permisos para administrar usuarios.");
        }
    }
}
