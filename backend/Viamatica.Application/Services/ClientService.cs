using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Clients;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyCollection<ClientResponseDto>> GetAllAsync(string? identification, CancellationToken cancellationToken = default)
        => _clientRepository.GetAllAsync(identification, cancellationToken);

    public async Task<ClientResponseDto> GetByIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(clientId, cancellationToken);
        return client ?? throw new NotFoundException($"No se encontró el cliente {clientId}.");
    }

    public async Task<ClientResponseDto> CreateAsync(CreateClientRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureUniqueAsync(request.Identification, request.Email, null, cancellationToken);

        var client = new Client(
            request.Name.Trim(),
            request.LastName.Trim(),
            request.Identification.Trim(),
            request.Email.Trim(),
            request.PhoneNumber.Trim(),
            request.Address.Trim(),
            request.ReferenceAddress.Trim());

        _clientRepository.Add(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(client.ClientId, cancellationToken);
    }

    public async Task<ClientResponseDto> UpdateAsync(int clientId, UpdateClientRequestDto request, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetForUpdateAsync(clientId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente {clientId}.");

        await EnsureUniqueAsync(request.Identification, request.Email, clientId, cancellationToken);

        client.Update(
            request.Name.Trim(),
            request.LastName.Trim(),
            request.Identification.Trim(),
            request.Email.Trim(),
            request.PhoneNumber.Trim(),
            request.Address.Trim(),
            request.ReferenceAddress.Trim());

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(client.ClientId, cancellationToken);
    }

    public async Task DeleteAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetForUpdateAsync(clientId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente {clientId}.");

        _clientRepository.Remove(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueAsync(string identification, string email, int? currentClientId, CancellationToken cancellationToken)
    {
        var identificationExists = await _clientRepository.IdentificationExistsAsync(identification.Trim(), currentClientId, cancellationToken);

        if (identificationExists)
        {
            throw new ConflictException("La identificación ya existe.");
        }

        var emailExists = await _clientRepository.EmailExistsAsync(email.Trim(), currentClientId, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("El email del cliente ya existe.");
        }
    }
}
