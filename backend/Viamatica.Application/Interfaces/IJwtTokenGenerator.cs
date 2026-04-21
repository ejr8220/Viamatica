using Viamatica.Application.DTOs;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IJwtTokenGenerator
{
    GeneratedTokenDto Generate(User user);
}
