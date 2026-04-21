namespace Viamatica.Domain.Entities;

public class AttentionType
{
    public string AttentionTypeId { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Attention> Attentions { get; private set; } = new List<Attention>();

    private AttentionType() { } // EF Constructor

    public AttentionType(string attentionTypeId, string description)
    {
        if (string.IsNullOrWhiteSpace(attentionTypeId))
            throw new ArgumentException("Attention type id is required", nameof(attentionTypeId));

        if (attentionTypeId.Length > 3)
            throw new ArgumentException("Attention type id must have a maximum length of 3", nameof(attentionTypeId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        AttentionTypeId = attentionTypeId;
        Description = description;
    }
}
