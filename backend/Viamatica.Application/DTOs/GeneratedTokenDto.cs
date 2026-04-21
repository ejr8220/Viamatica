namespace Viamatica.Application.DTOs;

public sealed class GeneratedTokenDto
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
