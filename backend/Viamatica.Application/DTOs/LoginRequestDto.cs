using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs;

public sealed class LoginRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
