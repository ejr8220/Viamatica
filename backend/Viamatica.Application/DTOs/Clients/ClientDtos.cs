using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Clients;

public class CreateClientRequestDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(13, MinimumLength = 10)]
    public string Identification { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(13, MinimumLength = 10)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 20)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 20)]
    public string ReferenceAddress { get; set; } = string.Empty;
}

public sealed class UpdateClientRequestDto : CreateClientRequestDto
{
}

public sealed class ClientResponseDto
{
    public int ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ReferenceAddress { get; set; } = string.Empty;
}
