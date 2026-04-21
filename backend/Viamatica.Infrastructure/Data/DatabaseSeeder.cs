using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Common;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Security;

namespace Viamatica.Infrastructure.Data;

public sealed class DatabaseSeeder
{
    private static readonly IReadOnlyDictionary<string, string> BaseUserIdentifications = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["admin2026"] = "0100000001",
        ["gestor01"] = "0100000002",
        ["cajero01"] = "0100000003"
    };

    private readonly ViamaticaDbContext _dbContext;
    private readonly DatabaseFieldProtector _databaseFieldProtector;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ViamaticaDbContext dbContext, IPasswordHasher passwordHasher, DatabaseFieldProtector databaseFieldProtector)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _databaseFieldProtector = databaseFieldProtector;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedStoredProceduresAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedNavigationMenuAsync(cancellationToken);
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
            new StatusContract(ContractStatusIds.Active, "Vigente"),
            new StatusContract(ContractStatusIds.Cancelled, "Cancelado"),
            new StatusContract(ContractStatusIds.Replaced, "Sustituido"),
            new StatusContract(ContractStatusIds.Renewed, "Renovación de servicio"));

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
            await NormalizeExistingUsersAsync(cancellationToken);
            return;
        }

        var admin = new User("admin2026", "0100000001", "admin@viamatica.local", _passwordHasher.Hash("Admin2026"), RoleIds.Administrator, UserStatusIds.Active);
        admin.Approve();

        var gestor = new User("gestor01", "0100000002", "gestor@viamatica.local", _passwordHasher.Hash("Gestor2026"), RoleIds.Gestor, UserStatusIds.Active);
        gestor.Approve();

        var cajero = new User("cajero01", "0100000003", "cajero@viamatica.local", _passwordHasher.Hash("Cajero2026"), RoleIds.Cashier, UserStatusIds.Active);
        cajero.Approve();

        _dbContext.Users.AddRange(admin, gestor, cajero);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task NormalizeExistingUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .ToListAsync(cancellationToken);

        if (users.Count == 0)
        {
            return;
        }

        var reservedIdentifications = new HashSet<string>(
            users
                .Select(user => user.Identification)
                .Where(identification => Regex.IsMatch(identification, @"^\d{10}$")),
            StringComparer.Ordinal);

        var rawStoredValues = await LoadRawStoredUserFieldsAsync(cancellationToken);

        foreach (var user in users)
        {
            var rawFields = rawStoredValues.GetValueOrDefault(user.UserId);
            var needsIdentificationNormalization = !Regex.IsMatch(user.Identification, @"^\d{10}$");
            var needsEmailRewrite = rawFields is not null && !_databaseFieldProtector.IsProtectedValue(rawFields.Email);
            var needsIdentificationRewrite = rawFields is not null && !_databaseFieldProtector.IsProtectedValue(rawFields.Identification);

            if (!needsIdentificationNormalization && !needsEmailRewrite && !needsIdentificationRewrite)
            {
                continue;
            }

            var finalIdentification = user.Identification;
            if (needsIdentificationNormalization)
            {
                var normalizedIdentification = ResolveIdentification(user.UserName, user.UserId, reservedIdentifications);
                finalIdentification = normalizedIdentification;
                reservedIdentifications.Add(normalizedIdentification);
            }

            await RewriteStoredUserFieldsAsync(user.UserId, finalIdentification, user.Email, cancellationToken);
        }
    }

    private async Task<Dictionary<int, RawStoredUserFields>> LoadRawStoredUserFieldsAsync(CancellationToken cancellationToken)
    {
        var result = new Dictionary<int, RawStoredUserFields>();
        var connection = _dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT userid, identification, email FROM usertable WHERE isdeleted = 0;";
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var userId = reader.GetInt32(0);
                var identification = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                var email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                result[userId] = new RawStoredUserFields(identification, email);
            }
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }

        return result;
    }

    private async Task RewriteStoredUserFieldsAsync(int userId, string identification, string email, CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE usertable
                SET identification = @identification,
                    email = @email,
                    identificationhash = @identificationhash,
                    emailhash = @emailhash
                WHERE userid = @userid;
                """;

            var identificationParameter = command.CreateParameter();
            identificationParameter.ParameterName = "@identification";
            identificationParameter.Value = _databaseFieldProtector.Protect(identification);
            command.Parameters.Add(identificationParameter);

            var emailParameter = command.CreateParameter();
            emailParameter.ParameterName = "@email";
            emailParameter.Value = _databaseFieldProtector.Protect(email);
            command.Parameters.Add(emailParameter);

            var identificationHashParameter = command.CreateParameter();
            identificationHashParameter.ParameterName = "@identificationhash";
            identificationHashParameter.Value = _databaseFieldProtector.ComputeHash(identification);
            command.Parameters.Add(identificationHashParameter);

            var emailHashParameter = command.CreateParameter();
            emailHashParameter.ParameterName = "@emailhash";
            emailHashParameter.Value = _databaseFieldProtector.ComputeHash(email);
            command.Parameters.Add(emailHashParameter);

            var userIdParameter = command.CreateParameter();
            userIdParameter.ParameterName = "@userid";
            userIdParameter.Value = userId;
            command.Parameters.Add(userIdParameter);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private sealed record RawStoredUserFields(string Identification, string Email);

    private static string ResolveIdentification(string userName, int userId, ISet<string> reservedIdentifications)
    {
        if (BaseUserIdentifications.TryGetValue(userName, out var fixedIdentification) && !reservedIdentifications.Contains(fixedIdentification))
        {
            return fixedIdentification;
        }

        var sequence = Math.Max(userId, 1);
        while (true)
        {
            var candidate = $"010{sequence:D7}";
            if (!reservedIdentifications.Contains(candidate))
            {
                return candidate;
            }

            sequence++;
        }
    }

    private async Task SeedNavigationMenuAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.NavigationMenus.AnyAsync(cancellationToken))
        {
            await NormalizeNavigationMenuAsync(cancellationToken);
            return;
        }

        var welcome = new NavigationMenu("welcome", "Bienvenida", "pi pi-home", "/welcome", 10);
        var dashboard = new NavigationMenu("dashboard", "Dashboard", "pi pi-chart-bar", "/dashboard", 20);
        var administration = new NavigationMenu("administration", "Administracion", "pi pi-cog", null, 30);
        var operation = new NavigationMenu("operation", "Operacion", "pi pi-briefcase", null, 40);
        var catalogs = new NavigationMenu("catalogs", "Catalogos", "pi pi-list", null, 50);
        var cashDesk = new NavigationMenu("cash-desk", "Caja", "pi pi-wallet", null, 60);
        var management = new NavigationMenu("management", "Gestion", "pi pi-sitemap", null, 70);

        _dbContext.NavigationMenus.AddRange(welcome, dashboard, administration, operation, catalogs, cashDesk, management);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var users = new NavigationMenu("users", "Usuarios", "pi pi-id-card", "/users", 10, administration.NavigationMenuId);
        var activeUsers = new NavigationMenu("active-users", "Reporte activos", "pi pi-table", "/users?view=active-report", 20, administration.NavigationMenuId);
        var clients = new NavigationMenu("clients", "Clientes", "pi pi-users", "/clients", 10, operation.NavigationMenuId);
        var contracts = new NavigationMenu("contracts", "Contratos", "pi pi-file-edit", "/contracts", 20, operation.NavigationMenuId);
        var attentions = new NavigationMenu("attentions", "Atenciones", "pi pi-comments", "/attentions", 30, operation.NavigationMenuId);
        var cashes = new NavigationMenu("cashes", "Cajas", "pi pi-shop", "/cashes", 10, catalogs.NavigationMenuId);
        var turns = new NavigationMenu("turns", "Turnos", "pi pi-ticket", "/turns", 20, catalogs.NavigationMenuId);
        var services = new NavigationMenu("services", "Servicios", "pi pi-box", "/services", 30, catalogs.NavigationMenuId);
        var cashSession = new NavigationMenu("cash-session", "Sesion de caja", "pi pi-play-circle", "/cashes", 10, cashDesk.NavigationMenuId);
        var cashProcesses = new NavigationMenu("cash-processes", "Procesos de caja", "pi pi-credit-card", "/attentions", 20, cashDesk.NavigationMenuId);
        var assignTurns = new NavigationMenu("assign-turns", "Asignacion turnos", "pi pi-send", "/turns", 10, management.NavigationMenuId);
        var managedUsers = new NavigationMenu("managed-users", "Usuarios", "pi pi-id-card", "/users", 20, management.NavigationMenuId);

        _dbContext.NavigationMenus.AddRange(
            users,
            activeUsers,
            clients,
            contracts,
            attentions,
            cashes,
            turns,
            services,
            cashSession,
            cashProcesses,
            assignTurns,
            managedUsers);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _dbContext.NavigationMenuRoles.AddRange(
            new NavigationMenuRole(welcome.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(welcome.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(welcome.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(dashboard.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(administration.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(users.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(activeUsers.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(operation.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(operation.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(operation.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(clients.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(clients.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(clients.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(contracts.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(contracts.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(contracts.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(attentions.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(attentions.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(catalogs.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(catalogs.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(cashes.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(cashes.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(cashes.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(turns.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(turns.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(services.NavigationMenuId, RoleIds.Administrator),
            new NavigationMenuRole(services.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(cashDesk.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(cashSession.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(cashProcesses.NavigationMenuId, RoleIds.Cashier),
            new NavigationMenuRole(management.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(assignTurns.NavigationMenuId, RoleIds.Gestor),
            new NavigationMenuRole(managedUsers.NavigationMenuId, RoleIds.Gestor));

        await _dbContext.SaveChangesAsync(cancellationToken);
        await NormalizeNavigationMenuAsync(cancellationToken);
    }

    private async Task NormalizeNavigationMenuAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.ExecuteSqlRawAsync(
            """
            UPDATE navigationmenu
            SET route = '/cashes'
            WHERE menukey = 'cash-session'
              AND ISNULL(route, '') <> '/cashes';

            UPDATE navigationmenu
            SET route = '/attentions'
            WHERE menukey = 'cash-processes'
              AND ISNULL(route, '') <> '/attentions';

            INSERT INTO navigationmenurol (navigationmenuid, rolid)
            SELECT navigationmenuid, 3
            FROM navigationmenu
            WHERE menukey IN ('cash-session', 'cash-processes')
              AND NOT EXISTS (
                  SELECT 1
                  FROM navigationmenurol
                  WHERE navigationmenurol.navigationmenuid = navigationmenu.navigationmenuid
                    AND navigationmenurol.rolid = 3
              );

            DELETE navigationmenurol
            FROM navigationmenurol
            INNER JOIN navigationmenu
                ON navigationmenu.navigationmenuid = navigationmenurol.navigationmenuid
            WHERE navigationmenu.menukey = 'attentions'
              AND navigationmenurol.rolid = 3;
            """,
            cancellationToken);
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
