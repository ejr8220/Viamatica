using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Services;

public class CreateServiceRequestDto
{
    [Required]
    [StringLength(50)]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string ServiceDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}

public sealed class UpdateServiceRequestDto : CreateServiceRequestDto
{
}

public sealed class ServiceResponseDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
