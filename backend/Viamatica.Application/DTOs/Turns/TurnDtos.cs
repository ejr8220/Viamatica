using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Turns;

public class CreateTurnRequestDto
{
    [Required]
    [RegularExpression("^[A-Z]{2}\\d{4}$")]
    public string Description { get; set; } = string.Empty;

    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    [Range(1, int.MaxValue)]
    public int CashId { get; set; }
}

public sealed class UpdateTurnRequestDto : CreateTurnRequestDto
{
}

public sealed class TurnResponseDto
{
    public int TurnId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }
    public int CashId { get; set; }
    public string CashDescription { get; set; } = string.Empty;
    public int UserGestorId { get; set; }
    public string GestorUserName { get; set; } = string.Empty;
}
