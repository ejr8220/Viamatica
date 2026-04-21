using System.Data;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.DTOs.Users;
using Viamatica.Application.Interfaces;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Queries;

public sealed class ActiveUserReportQuery : IActiveUserReportQuery
{
    private readonly ViamaticaDbContext _dbContext;

    public ActiveUserReportQuery(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ActiveUserReportDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<ActiveUserReportDto>();
        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.usp_GetActiveUsers";
            command.CommandType = CommandType.StoredProcedure;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new ActiveUserReportDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                    UserName = reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    RoleName = reader.GetString(reader.GetOrdinal("role_name")),
                    StatusDescription = reader.GetString(reader.GetOrdinal("status_description"))
                });
            }
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }

        return results;
    }
}
