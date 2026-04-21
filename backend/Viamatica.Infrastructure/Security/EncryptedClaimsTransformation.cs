using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Viamatica.Infrastructure.Security;

public sealed class EncryptedClaimsTransformation : IClaimsTransformation
{
    private readonly AesClaimsProtector _claimsProtector;

    public EncryptedClaimsTransformation(AesClaimsProtector claimsProtector)
    {
        _claimsProtector = claimsProtector;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
        {
            return Task.FromResult(principal);
        }

        if (identity.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
        {
            return Task.FromResult(principal);
        }

        AddDecryptedClaim(identity, "enc_sub", JwtRegisteredClaimNames.Sub);
        AddDecryptedClaim(identity, "enc_name_id", ClaimTypes.NameIdentifier);
        AddDecryptedClaim(identity, "enc_name", ClaimTypes.Name);
        AddDecryptedClaim(identity, "enc_unique_name", JwtRegisteredClaimNames.UniqueName);
        AddDecryptedClaim(identity, "enc_role", ClaimTypes.Role);

        return Task.FromResult(principal);
    }

    private void AddDecryptedClaim(ClaimsIdentity identity, string encryptedClaimType, string targetClaimType)
    {
        var encryptedValue = identity.FindFirst(encryptedClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(encryptedValue))
        {
            return;
        }

        var decryptedValue = _claimsProtector.Unprotect(encryptedValue);

        if (string.IsNullOrWhiteSpace(decryptedValue) || identity.HasClaim(claim => claim.Type == targetClaimType))
        {
            return;
        }

        identity.AddClaim(new Claim(targetClaimType, decryptedValue));
    }
}
