namespace Viamatica.Application.Interfaces;

public interface IPasswordHasher
{
    bool Verify(string plainTextPassword, string storedPassword);
    string Hash(string plainTextPassword);
}
