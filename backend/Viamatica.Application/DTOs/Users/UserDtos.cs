using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Users;

public sealed class CreateUserRequestDto
{
    [Required]
    [StringLength(20, MinimumLength = 8)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Range(2, 3)]
    public int RoleId { get; set; }
}

public sealed class UpdateUserRequestDto
{
    [Required]
    [StringLength(20, MinimumLength = 8)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30, MinimumLength = 8)]
    public string? Password { get; set; }

    [Range(2, 3)]
    public int RoleId { get; set; }

    public bool Active { get; set; } = true;
}

public sealed class UserResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string StatusId { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public DateTimeOffset? DateApproval { get; set; }
}

public sealed class ActiveUserReportDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
}
