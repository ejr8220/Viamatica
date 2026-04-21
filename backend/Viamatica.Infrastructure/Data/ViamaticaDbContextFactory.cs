using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Viamatica.Infrastructure.Data;

public class ViamaticaDbContextFactory : IDesignTimeDbContextFactory<ViamaticaDbContext>
{
    public ViamaticaDbContext CreateDbContext(string[] args)
    {
        // Build configuration from API project's appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Viamatica.API"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ViamaticaDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        optionsBuilder.UseSqlServer(connectionString, 
            b => b.MigrationsAssembly(typeof(ViamaticaDbContext).Assembly.FullName));

        return new ViamaticaDbContext(optionsBuilder.Options);
    }
}
