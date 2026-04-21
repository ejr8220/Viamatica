using Microsoft.Extensions.DependencyInjection;
using Viamatica.Application.Interfaces;
using Viamatica.Application.Services;

namespace Viamatica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ICashManagementService, CashManagementService>();
        services.AddScoped<ITurnService, TurnService>();
        services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IAttentionService, AttentionService>();
        return services;
    }
}
