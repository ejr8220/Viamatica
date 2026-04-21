using Viamatica.Application.DTOs.Users;

namespace Viamatica.Application.Interfaces;

public interface IActiveUserReportQuery
{
    Task<IReadOnlyCollection<ActiveUserReportDto>> GetAsync(CancellationToken cancellationToken = default);
}
