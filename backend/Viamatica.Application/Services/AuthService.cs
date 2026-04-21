using Viamatica.Application.Common;
using Viamatica.Application.DTOs;
using Viamatica.Application.Interfaces;

namespace Viamatica.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserAuthenticationRepository _userAuthenticationRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserAuthenticationRepository userAuthenticationRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userAuthenticationRepository = userAuthenticationRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var userNameOrEmail = request.UserNameOrEmail.Trim();
        var user = await _userAuthenticationRepository.GetByUserNameOrEmailAsync(userNameOrEmail, cancellationToken);

        if (user is null
            || !_passwordHasher.Verify(request.Password, user.Password)
            || user.UserApproval != 1
            || user.StatusId != UserStatusIds.Active)
        {
            return null;
        }

        var token = _jwtTokenGenerator.Generate(user);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            UserName = user.UserName,
            Role = user.Role?.RoleName ?? string.Empty
        };
    }

    public async Task<ForgotPasswordResponseDto> ResetPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var userNameOrEmail = request.UserNameOrEmail.Trim();
        var identification = request.Identification.Trim();

        var user = await _userAuthenticationRepository.GetByUserNameOrEmailAsync(userNameOrEmail, cancellationToken)
            ?? throw new NotFoundException("No existe un usuario asociado con la información proporcionada.");

        if (user.UserApproval != 1 || user.StatusId != UserStatusIds.Active)
        {
            throw new BusinessRuleException("Solo los usuarios activos y aprobados pueden recuperar su contraseña.");
        }

        if (!string.Equals(user.Identification, identification, StringComparison.Ordinal))
        {
            throw new ForbiddenOperationException("La identificación no coincide con el usuario indicado.");
        }

        user.ChangePassword(_passwordHasher.Hash(request.NewPassword.Trim()));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ForgotPasswordResponseDto
        {
            Message = "La contraseña se actualizó correctamente."
        };
    }
}
