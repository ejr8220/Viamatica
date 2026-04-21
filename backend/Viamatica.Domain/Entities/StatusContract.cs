namespace Viamatica.Domain.Entities;

public class StatusContract
{
    public string StatusId { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Contract> Contracts { get; private set; } = new List<Contract>();

    private StatusContract() { } // EF Constructor

    public StatusContract(string statusId, string description)
    {
        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentException("Status id is required", nameof(statusId));

        if (statusId.Length > 3)
            throw new ArgumentException("Status id must have a maximum length of 3", nameof(statusId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        StatusId = statusId;
        Description = description;
    }
}
