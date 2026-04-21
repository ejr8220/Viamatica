using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Cashes;

public sealed class CreateCashRequestDto
{
    [Required]
    [StringLength(50)]
    public string CashDescription { get; set; } = string.Empty;
}

public sealed class UpdateCashRequestDto
{
    [Required]
    [StringLength(50)]
    public string CashDescription { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[YN]$")]
    public string Active { get; set; } = "Y";
}

public sealed class CashAssignmentResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public sealed class CashResponseDto
{
    public int CashId { get; set; }
    public string CashDescription { get; set; } = string.Empty;
    public string Active { get; set; } = string.Empty;
    public List<CashAssignmentResponseDto> AssignedCashiers { get; set; } = [];
    public CashSessionResponseDto? ActiveSession { get; set; }
}

public sealed class CashSessionResponseDto
{
    public int CashSessionId { get; set; }
    public int CashId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public bool IsActive { get; set; }
}
