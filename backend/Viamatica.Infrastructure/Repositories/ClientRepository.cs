using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.DTOs.Clients;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class ClientRepository : IClientRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public ClientRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ClientResponseDto>> GetAllAsync(string? identification, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Clients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(identification))
        {
            query = query.Where(client => client.Identification == identification.Trim());
        }

        return await query
            .OrderBy(client => client.ClientId)
            .Select(Map)
            .ToListAsync(cancellationToken);
    }

    public Task<ClientResponseDto?> GetByIdAsync(int clientId, CancellationToken cancellationToken = default)
        => _dbContext.Clients
            .AsNoTracking()
            .Where(entity => entity.ClientId == clientId)
            .Select(Map)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Client?> GetForUpdateAsync(int clientId, CancellationToken cancellationToken = default)
        => _dbContext.Clients.FirstOrDefaultAsync(entity => entity.ClientId == clientId, cancellationToken);

    public Task<bool> IdentificationExistsAsync(string identification, int? currentClientId, CancellationToken cancellationToken = default)
        => _dbContext.Clients.AnyAsync(
            client => client.Identification == identification &&
                     (!currentClientId.HasValue || client.ClientId != currentClientId.Value),
            cancellationToken);

    public Task<bool> EmailExistsAsync(string email, int? currentClientId, CancellationToken cancellationToken = default)
        => _dbContext.Clients.AnyAsync(
            client => client.Email == email &&
                     (!currentClientId.HasValue || client.ClientId != currentClientId.Value),
            cancellationToken);

    public void Add(Client client) => _dbContext.Clients.Add(client);

    public void Remove(Client client) => _dbContext.Clients.Remove(client);

    private static readonly Expression<Func<Client, ClientResponseDto>> Map = client => new ClientResponseDto
    {
        ClientId = client.ClientId,
        Name = client.Name,
        LastName = client.LastName,
        Identification = client.Identification,
        Email = client.Email,
        PhoneNumber = client.PhoneNumber,
        Address = client.Address,
        ReferenceAddress = client.ReferenceAddress
    };
}
