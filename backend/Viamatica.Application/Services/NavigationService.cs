using Viamatica.Application.DTOs.Navigation;
using Viamatica.Application.Interfaces;

namespace Viamatica.Application.Services;

public sealed class NavigationService : INavigationService
{
    private readonly INavigationMenuQuery _navigationMenuQuery;

    public NavigationService(INavigationMenuQuery navigationMenuQuery)
    {
        _navigationMenuQuery = navigationMenuQuery;
    }

    public Task<IReadOnlyCollection<NavigationMenuItemDto>> GetMenuAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException("Role name is required.", nameof(roleName));
        }

        return _navigationMenuQuery.GetByRoleAsync(roleName.Trim(), cancellationToken);
    }
}
