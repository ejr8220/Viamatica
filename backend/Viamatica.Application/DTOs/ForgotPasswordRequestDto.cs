using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs;

public sealed class ForgotPasswordRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{10,13}$")]
    public string Identification { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;
}
