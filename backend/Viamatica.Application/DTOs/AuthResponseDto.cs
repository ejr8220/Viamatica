namespace Viamatica.Application.DTOs;

public sealed class AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAtUtc { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
