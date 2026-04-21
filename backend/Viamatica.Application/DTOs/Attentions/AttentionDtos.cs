using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Attentions;

public sealed class StartAttentionRequestDto
{
    [Range(1, int.MaxValue)]
    public int TurnId { get; set; }

    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    public int? ContractId { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string AttentionTypeId { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Notes { get; set; }
}

public sealed class CloseAttentionRequestDto
{
    [StringLength(300)]
    public string? Notes { get; set; }
}

public sealed class AttentionTypeResponseDto
{
    public string AttentionTypeId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class AttentionResponseDto
{
    public int AttentionId { get; set; }
    public int TurnId { get; set; }
    public string TurnDescription { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int? ContractId { get; set; }
    public int CashierUserId { get; set; }
    public string CashierUserName { get; set; } = string.Empty;
    public string AttentionTypeId { get; set; } = string.Empty;
    public string AttentionTypeDescription { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
