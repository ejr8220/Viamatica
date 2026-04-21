using Microsoft.EntityFrameworkCore;
using Viamatica.Application.DTOs.Navigation;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Queries;

public sealed class NavigationMenuQuery : INavigationMenuQuery
{
    private readonly ViamaticaDbContext _dbContext;

    public NavigationMenuQuery(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<NavigationMenuItemDto>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Set<NavigationMenu>()
            .AsNoTracking()
            .Where(menu =>
                menu.IsActive &&
                menu.NavigationMenuRoles.Any(role => role.Role.RoleName == roleName))
            .Select(menu => new FlatNavigationMenuItem
            {
                NavigationMenuId = menu.NavigationMenuId,
                ParentNavigationMenuId = menu.ParentNavigationMenuId,
                MenuKey = menu.MenuKey,
                Label = menu.Label,
                Icon = menu.Icon,
                Route = menu.Route,
                DisplayOrder = menu.DisplayOrder
            })
            .OrderBy(menu => menu.DisplayOrder)
            .ThenBy(menu => menu.NavigationMenuId)
            .ToListAsync(cancellationToken);

        return items
            .Where(menu => menu.ParentNavigationMenuId is null)
            .OrderBy(menu => menu.DisplayOrder)
            .ThenBy(menu => menu.NavigationMenuId)
            .Select(menu => Map(menu, items))
            .ToList();
    }

    private static NavigationMenuItemDto Map(FlatNavigationMenuItem item, IReadOnlyCollection<FlatNavigationMenuItem> items)
    {
        var children = items
            .Where(child => child.ParentNavigationMenuId == item.NavigationMenuId)
            .OrderBy(child => child.DisplayOrder)
            .ThenBy(child => child.NavigationMenuId)
            .Select(child => Map(child, items))
            .ToList();

        return new NavigationMenuItemDto
        {
            NavigationMenuId = item.NavigationMenuId,
            MenuKey = item.MenuKey,
            Label = item.Label,
            Icon = item.Icon,
            Route = item.Route,
            DisplayOrder = item.DisplayOrder,
            Children = children
        };
    }

    private sealed class FlatNavigationMenuItem
    {
        public int NavigationMenuId { get; init; }
        public int? ParentNavigationMenuId { get; init; }
        public string MenuKey { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
        public string? Route { get; init; }
        public int DisplayOrder { get; init; }
    }
}
