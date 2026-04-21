namespace Viamatica.Domain.Entities;

public class Attention
{
    public int AttentionId { get; private set; }
    public int TurnId { get; private set; }
    public int ClientId { get; private set; }
    public int? ContractId { get; private set; }
    public int CashierUserId { get; private set; }
    public string AttentionTypeId { get; private set; } = string.Empty;
    public int StatusId { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    // Navigation
    public Turn Turn { get; private set; } = null!;
    public Client Client { get; private set; } = null!;
    public Contract? Contract { get; private set; }
    public User CashierUser { get; private set; } = null!;
    public AttentionType AttentionType { get; private set; } = null!;
    public AttentionStatus Status { get; private set; } = null!;

    private Attention() { } // EF Constructor

    public Attention(int turnId, int clientId, int? contractId, int cashierUserId, string attentionTypeId, int statusId, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(attentionTypeId))
            throw new ArgumentException("Attention type id is required", nameof(attentionTypeId));

        if (attentionTypeId.Length > 3)
            throw new ArgumentException("Attention type id must have a maximum length of 3", nameof(attentionTypeId));

        TurnId = turnId;
        ClientId = clientId;
        ContractId = contractId;
        CashierUserId = cashierUserId;
        AttentionTypeId = attentionTypeId;
        StatusId = statusId;
        Notes = notes?.Trim() ?? string.Empty;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete(string? notes = null)
    {
        StatusId = 2;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel(string? notes = null)
    {
        StatusId = 3;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
        CompletedAt = DateTimeOffset.UtcNow;
    }
}
