using Viamatica.Application.Interfaces;

namespace Viamatica.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainTextPassword) => BCrypt.Net.BCrypt.HashPassword(plainTextPassword);

    public bool Verify(string plainTextPassword, string storedPassword)
    {
        if (string.IsNullOrWhiteSpace(storedPassword))
        {
            return false;
        }

        return storedPassword.StartsWith("$2", StringComparison.Ordinal)
            ? BCrypt.Net.BCrypt.Verify(plainTextPassword, storedPassword)
            : string.Equals(plainTextPassword, storedPassword, StringComparison.Ordinal);
    }
}
