using System.Text.RegularExpressions;

namespace Viamatica.Domain.Entities;

public class Turn
{
    public int TurnId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTimeOffset Date { get; private set; }
    public int CashId { get; private set; }
    public int UserGestorId { get; private set; }

    // Navigation
    public Cash Cash { get; private set; } = null!;
    public User UserGestor { get; private set; } = null!;
    public ICollection<Attention> Attentions { get; private set; } = new List<Attention>();

    private Turn() { } // EF Constructor

    public Turn(string description, DateTimeOffset date, int cashId, int userGestorId)
    {
        ValidateDescription(description);

        Description = description;
        Date = date;
        CashId = cashId;
        UserGestorId = userGestorId;
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Turn description is required", nameof(description));

        // 2 letras mayúsculas + 4 números (longitud 6)
        if (description.Length != 6)
            throw new ArgumentException("Turn description must be exactly 6 characters", nameof(description));

        if (!Regex.IsMatch(description, @"^[A-Z]{2}\d{4}$"))
            throw new ArgumentException("Turn description must be 2 uppercase letters followed by 4 numbers", nameof(description));
    }
}
