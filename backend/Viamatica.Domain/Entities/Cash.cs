namespace Viamatica.Domain.Entities;

public class Cash
{
    public int CashId { get; private set; }
    public string CashDescription { get; private set; } = string.Empty;
    public string Active { get; private set; } = string.Empty;

    // Navigation
    public ICollection<UserCash> UserCashes { get; private set; } = new List<UserCash>();
    public ICollection<Turn> Turns { get; private set; } = new List<Turn>();
    public ICollection<CashSession> Sessions { get; private set; } = new List<CashSession>();

    private Cash() { } // EF Constructor

    public Cash(string cashDescription, string active = "Y")
    {
        if (string.IsNullOrWhiteSpace(cashDescription))
            throw new ArgumentException("Cash description is required", nameof(cashDescription));

        if (string.IsNullOrWhiteSpace(active) || active.Length != 1)
            throw new ArgumentException("Active must contain a single character", nameof(active));

        CashDescription = cashDescription;
        Active = active.ToUpperInvariant();
    }

    public void Update(string cashDescription, string active)
    {
        if (string.IsNullOrWhiteSpace(cashDescription))
            throw new ArgumentException("Cash description is required", nameof(cashDescription));

        if (string.IsNullOrWhiteSpace(active) || active.Length != 1)
            throw new ArgumentException("Active must contain a single character", nameof(active));

        CashDescription = cashDescription;
        Active = active.ToUpperInvariant();
    }

    public void Activate() => Active = "Y";
    public void Deactivate() => Active = "N";
}
