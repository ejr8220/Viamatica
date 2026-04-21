namespace Viamatica.Domain.Entities;

public class NavigationMenu
{
    public int NavigationMenuId { get; private set; }
    public string MenuKey { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public string? Route { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public int? ParentNavigationMenuId { get; private set; }

    // Navigation
    public NavigationMenu? ParentNavigationMenu { get; private set; }
    public ICollection<NavigationMenu> Children { get; private set; } = new List<NavigationMenu>();
    public ICollection<NavigationMenuRole> NavigationMenuRoles { get; private set; } = new List<NavigationMenuRole>();

    private NavigationMenu() { }

    public NavigationMenu(
        string menuKey,
        string label,
        string icon,
        string? route,
        int displayOrder,
        int? parentNavigationMenuId = null,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(menuKey))
            throw new ArgumentException("Menu key is required.", nameof(menuKey));

        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Menu label is required.", nameof(label));

        if (string.IsNullOrWhiteSpace(icon))
            throw new ArgumentException("Menu icon is required.", nameof(icon));

        MenuKey = menuKey.Trim();
        Label = label.Trim();
        Icon = icon.Trim();
        Route = string.IsNullOrWhiteSpace(route) ? null : route.Trim();
        DisplayOrder = displayOrder;
        ParentNavigationMenuId = parentNavigationMenuId;
        IsActive = isActive;
    }
}
