namespace Viamatica.Application.DTOs.Navigation;

public sealed class NavigationMenuItemDto
{
    public int NavigationMenuId { get; init; }
    public string MenuKey { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<NavigationMenuItemDto> Children { get; init; } = [];
}
