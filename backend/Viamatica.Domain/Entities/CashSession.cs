namespace Viamatica.Domain.Entities;

public class CashSession
{
    public int CashSessionId { get; private set; }
    public int CashId { get; private set; }
    public int UserId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }

    public Cash Cash { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private CashSession()
    {
    }

    public CashSession(int cashId, int userId)
    {
        CashId = cashId;
        UserId = userId;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public bool IsActive => EndedAt is null;

    public void Close()
    {
        EndedAt = DateTimeOffset.UtcNow;
    }
}
