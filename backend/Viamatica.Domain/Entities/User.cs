using System.Text.RegularExpressions;
using Viamatica.Domain.Common;

namespace Viamatica.Domain.Entities;

public class User : SoftDeletableEntity
{
    public int UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public int RoleId { get; private set; }
    public string StatusId { get; private set; } = string.Empty;
    public int UserApproval { get; private set; }
    public DateTimeOffset? DateApproval { get; private set; }

    // Navigation
    public Role Role { get; private set; } = null!;
    public UserStatus Status { get; private set; } = null!;
    public ICollection<UserCash> UserCashes { get; private set; } = new List<UserCash>();
    public ICollection<Turn> Turns { get; private set; } = new List<Turn>();
    public ICollection<CashSession> CashSessions { get; private set; } = new List<CashSession>();
    public ICollection<Attention> CashierAttentions { get; private set; } = new List<Attention>();

    private User() { } // EF Constructor

    public User(string userName, string email, string password, int roleId, string statusId)
    {
        ValidateUserName(userName);
        ValidatePassword(password);
        ValidateEmail(email);
        ValidateStatusId(statusId);

        UserName = userName;
        Email = email;
        Password = password;
        RoleId = roleId;
        StatusId = statusId;
        UserApproval = 0;
    }

    public void UpdateProfile(string userName, string email, int roleId)
    {
        ValidateUserName(userName);
        ValidateEmail(email);

        UserName = userName;
        Email = email;
        RoleId = roleId;
    }

    public void ChangePassword(string password)
    {
        ValidatePassword(password);
        Password = password;
    }

    private static void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username is required", nameof(userName));

        if (userName.Length < 8 || userName.Length > 20)
            throw new ArgumentException("Username must be between 8 and 20 characters", nameof(userName));

        // Letras y al menos un número, sin caracteres especiales
        if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9]+$"))
            throw new ArgumentException("Username must contain only letters and numbers", nameof(userName));

        if (!Regex.IsMatch(userName, @"\d"))
            throw new ArgumentException("Username must contain at least one number", nameof(userName));
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required", nameof(password));

        if (password.StartsWith("$2", StringComparison.Ordinal))
            return;

        if (password.Length < 8 || password.Length > 30)
            throw new ArgumentException("Password must be between 8 and 30 characters", nameof(password));

        if (!Regex.IsMatch(password, @"[A-Z]"))
            throw new ArgumentException("Password must contain at least one uppercase letter", nameof(password));

        if (!Regex.IsMatch(password, @"[a-z]"))
            throw new ArgumentException("Password must contain at least one lowercase letter", nameof(password));

        if (!Regex.IsMatch(password, @"\d"))
            throw new ArgumentException("Password must contain at least one number", nameof(password));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email format", nameof(email));
    }

    private static void ValidateStatusId(string statusId)
    {
        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentException("Status id is required", nameof(statusId));

        if (statusId.Length > 3)
            throw new ArgumentException("Status id must have a maximum length of 3", nameof(statusId));
    }

    public void Approve()
    {
        UserApproval = 1;
        StatusId = "ACT";
        DateApproval = DateTimeOffset.UtcNow;
    }

    public void MarkPendingApproval()
    {
        UserApproval = 0;
        StatusId = "PEN";
        DateApproval = null;
    }

    public void Deactivate()
    {
        StatusId = "INA";
    }

    public void Activate()
    {
        StatusId = "ACT";
    }
}
