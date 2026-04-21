using Viamatica.Application.DTOs;

namespace Viamatica.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<ForgotPasswordResponseDto> ResetPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);
}
