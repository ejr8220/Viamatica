using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viamatica.Application.Interfaces;
using Viamatica.Infrastructure.Data;
using Viamatica.Infrastructure.Queries;
using Viamatica.Infrastructure.Repositories;
using Viamatica.Infrastructure.Security;

namespace Viamatica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ViamaticaDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(ViamaticaDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserAuthenticationRepository, UserAuthenticationRepository>();
        services.AddScoped<IUserManagementRepository, UserManagementRepository>();
        services.AddScoped<IActiveUserReportQuery, ActiveUserReportQuery>();
        services.AddScoped<INavigationMenuQuery, NavigationMenuQuery>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ICashManagementRepository, CashManagementRepository>();
        services.AddScoped<ITurnRepository, TurnRepository>();
        services.AddScoped<IServiceCatalogRepository, ServiceCatalogRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IAttentionRepository, AttentionRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<AesClaimsProtector>();
        services.AddSingleton<DatabaseFieldProtector>();
        services.AddTransient<IClaimsTransformation, EncryptedClaimsTransformation>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
