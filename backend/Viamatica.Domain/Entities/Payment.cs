namespace Viamatica.Domain.Entities;

public class Payment
{
    public int PaymentId { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public int ClientId { get; private set; }

    // Navigation
    public Client Client { get; private set; } = null!;

    private Payment() { } // EF Constructor

    public Payment(DateTimeOffset paymentDate, int clientId)
    {
        PaymentDate = paymentDate;
        ClientId = clientId;
    }
}
