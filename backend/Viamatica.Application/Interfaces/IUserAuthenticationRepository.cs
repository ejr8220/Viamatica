using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IUserAuthenticationRepository
{
    Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default);
}
