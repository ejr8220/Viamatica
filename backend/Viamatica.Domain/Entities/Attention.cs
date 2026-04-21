namespace Viamatica.Domain.Entities;

public class Attention
{
    public int AttentionId { get; private set; }
    public int TurnId { get; private set; }
    public int ClientId { get; private set; }
    public string AttentionTypeId { get; private set; } = string.Empty;
    public int StatusId { get; private set; }

    // Navigation
    public Turn Turn { get; private set; } = null!;
    public Client Client { get; private set; } = null!;
    public AttentionType AttentionType { get; private set; } = null!;
    public AttentionStatus Status { get; private set; } = null!;

    private Attention() { } // EF Constructor

    public Attention(int turnId, int clientId, string attentionTypeId, int statusId)
    {
        if (string.IsNullOrWhiteSpace(attentionTypeId))
            throw new ArgumentException("Attention type id is required", nameof(attentionTypeId));

        if (attentionTypeId.Length > 3)
            throw new ArgumentException("Attention type id must have a maximum length of 3", nameof(attentionTypeId));

        TurnId = turnId;
        ClientId = clientId;
        AttentionTypeId = attentionTypeId;
        StatusId = statusId;
    }
}
