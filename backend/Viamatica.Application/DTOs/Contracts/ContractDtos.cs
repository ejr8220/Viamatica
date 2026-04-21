using System.ComponentModel.DataAnnotations;

namespace Viamatica.Application.DTOs.Contracts;

public sealed class CreateContractRequestDto
{
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow.AddMonths(12);

    [Range(1, int.MaxValue)]
    public int ServiceId { get; set; }

    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    [Range(1, int.MaxValue)]
    public int MethodPaymentId { get; set; }
}

public sealed class UpdateContractRequestDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}

public sealed class ChangeContractServiceRequestDto
{
    [Range(1, int.MaxValue)]
    public int NewServiceId { get; set; }
}

public sealed class ChangeContractPaymentMethodRequestDto
{
    [Range(1, int.MaxValue)]
    public int MethodPaymentId { get; set; }
}

public sealed class CreatePaymentRequestDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class PaymentMethodResponseDto
{
    public int MethodPaymentId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public int ContractId { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset PaymentDate { get; set; }
}

public sealed class ContractResponseDto
{
    public int ContractId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string StatusId { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int MethodPaymentId { get; set; }
    public string MethodPaymentDescription { get; set; } = string.Empty;
    public List<PaymentResponseDto> Payments { get; set; } = [];
}
