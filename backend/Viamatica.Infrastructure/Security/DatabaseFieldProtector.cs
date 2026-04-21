using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Viamatica.Application.Configuration;

namespace Viamatica.Infrastructure.Security;

public sealed class DatabaseFieldProtector
{
    private readonly byte[] _key;

    public DatabaseFieldProtector(IOptions<JwtSettings> jwtOptions)
    {
        var encryptionKey = jwtOptions.Value.ClaimsEncryptionKey;

        if (string.IsNullOrWhiteSpace(encryptionKey))
        {
            throw new InvalidOperationException("JwtSettings:ClaimsEncryptionKey is required.");
        }

        _key = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));
    }

    public string Protect(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var plaintext = Encoding.UTF8.GetBytes(value);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var cipherText = new byte[plaintext.Length];
        var tag = new byte[16];

        using var aesGcm = new AesGcm(_key, 16);
        aesGcm.Encrypt(nonce, plaintext, cipherText, tag);

        var payload = new byte[nonce.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, payload, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, payload, nonce.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(payload);
    }

    public string Unprotect(string protectedValue)
    {
        if (string.IsNullOrWhiteSpace(protectedValue))
        {
            return string.Empty;
        }

        byte[] payload;
        try
        {
            payload = Convert.FromBase64String(protectedValue);
        }
        catch (FormatException)
        {
            // Legacy rows may still contain plaintext values from before field-level encryption.
            return protectedValue;
        }

        if (payload.Length < 29)
        {
            return protectedValue;
        }

        var nonce = payload[..12];
        var tag = payload[12..28];
        var cipherText = payload[28..];
        var plaintext = new byte[cipherText.Length];

        try
        {
            using var aesGcm = new AesGcm(_key, 16);
            aesGcm.Decrypt(nonce, cipherText, tag, plaintext);
        }
        catch (CryptographicException)
        {
            return protectedValue;
        }

        return Encoding.UTF8.GetString(plaintext);
    }

    public string ComputeHash(string value)
    {
        var normalizedValue = value.Trim().ToUpperInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedValue));
        return Convert.ToHexString(hash);
    }

    public bool IsProtectedValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return !string.Equals(Unprotect(value), value, StringComparison.Ordinal);
    }
}
