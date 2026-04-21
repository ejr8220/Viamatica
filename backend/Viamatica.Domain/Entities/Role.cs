namespace Viamatica.Domain.Entities;

public class Role
{
    public int RoleId { get; private set; }
    public string RoleName { get; private set; } = string.Empty;

    // Navigation
    public ICollection<User> Users { get; private set; } = new List<User>();

    private Role() { } // EF Constructor

    public Role(int roleId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name is required", nameof(roleName));

        RoleId = roleId;
        RoleName = roleName;
    }
}
