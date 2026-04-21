namespace Viamatica.Domain.Entities;

public class NavigationMenuRole
{
    public int NavigationMenuId { get; private set; }
    public int RoleId { get; private set; }

    // Navigation
    public NavigationMenu NavigationMenu { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    private NavigationMenuRole()
    {
    }

    public NavigationMenuRole(int navigationMenuId, int roleId)
    {
        NavigationMenuId = navigationMenuId;
        RoleId = roleId;
    }
}
