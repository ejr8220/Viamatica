using Viamatica.Application.DTOs.Navigation;

namespace Viamatica.Application.Interfaces;

public interface INavigationMenuQuery
{
    Task<IReadOnlyCollection<NavigationMenuItemDto>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);
}
