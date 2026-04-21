using Viamatica.Application.DTOs.Navigation;

namespace Viamatica.Application.Interfaces;

public interface INavigationService
{
    Task<IReadOnlyCollection<NavigationMenuItemDto>> GetMenuAsync(string roleName, CancellationToken cancellationToken = default);
}
