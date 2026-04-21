namespace Viamatica.Domain.Entities;

public class MethodPayment
{
    public int MethodPaymentId { get; private set; }
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Contract> Contracts { get; private set; } = new List<Contract>();

    private MethodPayment() { } // EF Constructor

    public MethodPayment(int methodPaymentId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        MethodPaymentId = methodPaymentId;
        Description = description;
    }
}
