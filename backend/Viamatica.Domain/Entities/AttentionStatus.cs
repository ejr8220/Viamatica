namespace Viamatica.Domain.Entities;

public class AttentionStatus
{
    public int StatusId { get; private set; }
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Attention> Attentions { get; private set; } = new List<Attention>();

    private AttentionStatus() { } // EF Constructor

    public AttentionStatus(int statusId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        StatusId = statusId;
        Description = description;
    }
}
