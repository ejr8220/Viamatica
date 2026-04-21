using Viamatica.Application.Common;
using Viamatica.Application.DTOs;
using Viamatica.Application.Interfaces;

namespace Viamatica.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserAuthenticationRepository _userAuthenticationRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserAuthenticationRepository userAuthenticationRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userAuthenticationRepository = userAuthenticationRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
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
}
