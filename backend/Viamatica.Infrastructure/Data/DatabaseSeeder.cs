using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Common;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data;

public sealed class DatabaseSeeder
{
    private readonly ViamaticaDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ViamaticaDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedStoredProceduresAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedUserStatusesAsync(cancellationToken);
        await SeedContractStatusesAsync(cancellationToken);
        await SeedPaymentMethodsAsync(cancellationToken);
        await SeedAttentionStatusesAsync(cancellationToken);
        await SeedAttentionTypesAsync(cancellationToken);
        await SeedUsersAsync(cancellationToken);
        await SeedBaseDataAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Roles.AnyAsync(cancellationToken))
        {
            return;
        }

        await _dbContext.Database.ExecuteSqlRawAsync("""
            SET IDENTITY_INSERT rol ON;
            INSERT INTO rol (rolid, rolname) VALUES (1, N'Administrador');
            INSERT INTO rol (rolid, rolname) VALUES (2, N'Gestor');
            INSERT INTO rol (rolid, rolname) VALUES (3, N'Cajero');
            SET IDENTITY_INSERT rol OFF;
            """);
    }

    private async Task SeedUserStatusesAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.UserStatuses.AnyAsync(cancellationToken))
        {
            return;
        }

        _dbContext.UserStatuses.AddRange(
            new UserStatus(UserStatusIds.Active, "Activo"),
            new UserStatus(UserStatusIds.Pending, "Pendiente de aprobación"),
            new UserStatus(UserStatusIds.Inactive, "Inactivo"));

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedContractStatusesAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.StatusContracts.AnyAsync(cancellationToken))
        {
            return;
        }

        _dbContext.StatusContracts.AddRange(
            new StatusContract(ContractStatusIds.Active, "Activo"),
            new StatusContract(ContractStatusIds.Cancelled, "Cancelado"),
            new StatusContract(ContractStatusIds.Replaced, "Reemplazado"));

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPaymentMethodsAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.MethodPayments.AnyAsync(cancellationToken))
        {
            return;
        }

        await _dbContext.Database.ExecuteSqlRawAsync("""
            SET IDENTITY_INSERT methodpayment ON;
            INSERT INTO methodpayment (methodpaymentid, description) VALUES (1, N'Efectivo');
            INSERT INTO methodpayment (methodpaymentid, description) VALUES (2, N'Tarjeta');
            INSERT INTO methodpayment (methodpaymentid, description) VALUES (3, N'Transferencia');
            SET IDENTITY_INSERT methodpayment OFF;
            """);
    }

    private async Task SeedAttentionStatusesAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.AttentionStatuses.AnyAsync(cancellationToken))
        {
            return;
        }

        await _dbContext.Database.ExecuteSqlRawAsync("""
            SET IDENTITY_INSERT attentionstatus ON;
            INSERT INTO attentionstatus (statusid, description) VALUES (1, N'Abierta');
            INSERT INTO attentionstatus (statusid, description) VALUES (2, N'Completada');
            INSERT INTO attentionstatus (statusid, description) VALUES (3, N'Cancelada');
            SET IDENTITY_INSERT attentionstatus OFF;
            """);
    }

    private async Task SeedAttentionTypesAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.AttentionTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        _dbContext.AttentionTypes.AddRange(
            new AttentionType(AttentionTypeIds.General, "Atención general"),
            new AttentionType(AttentionTypeIds.Contract, "Contratación"),
            new AttentionType(AttentionTypeIds.Payment, "Pago"),
            new AttentionType(AttentionTypeIds.ChangeService, "Cambio de servicio"),
            new AttentionType(AttentionTypeIds.ChangePayment, "Cambio de forma de pago"),
            new AttentionType(AttentionTypeIds.Cancellation, "Cancelación"));

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var admin = new User("admin2026", "admin@viamatica.local", _passwordHasher.Hash("Admin2026"), RoleIds.Administrator, UserStatusIds.Active);
        admin.Approve();

        var gestor = new User("gestor01", "gestor@viamatica.local", _passwordHasher.Hash("Gestor2026"), RoleIds.Gestor, UserStatusIds.Active);
        gestor.Approve();

        var cajero = new User("cajero01", "cajero@viamatica.local", _passwordHasher.Hash("Cajero2026"), RoleIds.Cashier, UserStatusIds.Active);
        cajero.Approve();

        _dbContext.Users.AddRange(admin, gestor, cajero);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedBaseDataAsync(CancellationToken cancellationToken)
    {
        if (!await _dbContext.Cashes.AnyAsync(cancellationToken))
        {
            _dbContext.Cashes.Add(new Cash("Caja principal"));
        }

        if (!await _dbContext.Services.AnyAsync(cancellationToken))
        {
            _dbContext.Services.Add(new Service("Internet Hogar", "Plan de internet residencial base para pruebas", 29.99m));
        }

        if (!await _dbContext.Clients.AnyAsync(cancellationToken))
        {
            _dbContext.Clients.Add(new Client(
                "Cliente",
                "Demo",
                "0102030405",
                "cliente.demo@viamatica.local",
                "0991234567",
                "Avenida Principal 123 y Calle Secundaria",
                "Frente al parque central de la ciudad"));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var firstCash = await _dbContext.Cashes.OrderBy(entity => entity.CashId).FirstAsync(cancellationToken);
        var cashier = await _dbContext.Users.FirstAsync(user => user.RoleId == RoleIds.Cashier, cancellationToken);

        var assignmentExists = await _dbContext.UserCashes.AnyAsync(entity => entity.CashId == firstCash.CashId && entity.UserId == cashier.UserId, cancellationToken);
        if (!assignmentExists)
        {
            _dbContext.UserCashes.Add(new UserCash(cashier.UserId, firstCash.CashId));
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SeedStoredProceduresAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("""
            CREATE OR ALTER PROCEDURE dbo.usp_GetActiveUsers
            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT
                    u.userid,
                    u.username,
                    u.email,
                    r.rolname AS role_name,
                    us.description AS status_description
                FROM dbo.usertable AS u
                INNER JOIN dbo.rol AS r ON r.rolid = u.rol_rolid
                INNER JOIN dbo.userstatus AS us ON us.statusid = u.userstatus_statusid
                WHERE u.isdeleted = 0
                  AND u.userstatus_statusid = 'ACT'
                  AND u.userapproval = 1;
            END;
            """, cancellationToken);
    }
}
