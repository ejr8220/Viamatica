namespace Viamatica.Domain.Entities;

public class Payment
{
    public int PaymentId { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public int ClientId { get; private set; }
    public int ContractId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public Client Client { get; private set; } = null!;
    public Contract Contract { get; private set; } = null!;

    private Payment() { } // EF Constructor

    public Payment(DateTimeOffset paymentDate, int clientId, int contractId, decimal amount, string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        PaymentDate = paymentDate;
        ClientId = clientId;
        ContractId = contractId;
        Amount = amount;
        Description = description?.Trim() ?? string.Empty;
    }
}
