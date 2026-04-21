namespace Viamatica.Domain.Entities;

public class UserCash
{
    public int UserId { get; private set; }
    public int CashId { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;
    public Cash Cash { get; private set; } = null!;

    private UserCash() { } // EF Constructor

    public UserCash(int userId, int cashId)
    {
        UserId = userId;
        CashId = cashId;
    }
}
