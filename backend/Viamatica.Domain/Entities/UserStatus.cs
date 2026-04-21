namespace Viamatica.Domain.Entities;

public class UserStatus
{
    public string StatusId { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public ICollection<User> Users { get; private set; } = new List<User>();

    private UserStatus() { } // EF Constructor

    public UserStatus(string statusId, string description)
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
