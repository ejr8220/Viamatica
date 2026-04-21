using Viamatica.Domain.Common;

namespace Viamatica.Domain.Entities;

public class Contract : SoftDeletableEntity
{
    public int ContractId { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public int ServiceId { get; private set; }
    public string StatusId { get; private set; } = string.Empty;
    public int ClientId { get; private set; }
    public int MethodPaymentId { get; private set; }

    // Navigation
    public Service Service { get; private set; } = null!;
    public StatusContract Status { get; private set; } = null!;
    public Client Client { get; private set; } = null!;
    public MethodPayment MethodPayment { get; private set; } = null!;
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    public ICollection<Attention> Attentions { get; private set; } = new List<Attention>();

    private Contract() { } // EF Constructor

    public Contract(DateTimeOffset startDate, DateTimeOffset endDate, int serviceId, 
                    string statusId, int clientId, int methodPaymentId)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentException("Status id is required", nameof(statusId));

        if (statusId.Length > 3)
            throw new ArgumentException("Status id must have a maximum length of 3", nameof(statusId));

        StartDate = startDate;
        EndDate = endDate;
        ServiceId = serviceId;
        StatusId = statusId;
        ClientId = clientId;
        MethodPaymentId = methodPaymentId;
    }

    public void UpdateDates(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        StartDate = startDate;
        EndDate = endDate;
    }

    public void ChangePaymentMethod(int methodPaymentId)
    {
        MethodPaymentId = methodPaymentId;
    }

    public void ChangeService(int serviceId)
    {
        ServiceId = serviceId;
    }

    public void Activate()
    {
        StatusId = "ACT";
    }

    public void Cancel()
    {
        StatusId = "CAN";
    }

    public void Cancel(DateTimeOffset cancellationDate)
    {
        StatusId = "CAN";
        EndDate = cancellationDate;
    }

    public void MarkAsReplaced()
    {
        StatusId = "REP";
    }

    public void MarkAsRenewed()
    {
        StatusId = "REN";
    }

    public bool IsOperative()
        => StatusId is "ACT" or "REN";
}
