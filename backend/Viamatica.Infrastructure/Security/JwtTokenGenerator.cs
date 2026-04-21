using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Viamatica.Application.Configuration;
using Viamatica.Application.DTOs;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly AesClaimsProtector _claimsProtector;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions, AesClaimsProtector claimsProtector)
    {
        _jwtSettings = jwtOptions.Value;
        _claimsProtector = claimsProtector;
    }

    public GeneratedTokenDto Generate(User user)
    {
        if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
        {
            throw new InvalidOperationException("JwtSettings:SecretKey is required.");
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("enc_sub", _claimsProtector.Protect(user.UserId.ToString(CultureInfo.InvariantCulture))),
            new("enc_name_id", _claimsProtector.Protect(user.UserId.ToString(CultureInfo.InvariantCulture))),
            new("enc_name", _claimsProtector.Protect(user.UserName)),
            new("enc_unique_name", _claimsProtector.Protect(user.UserName)),
            new("enc_email", _claimsProtector.Protect(user.Email)),
            new("enc_status_id", _claimsProtector.Protect(user.StatusId)),
            new("enc_role_id", _claimsProtector.Protect(user.RoleId.ToString(CultureInfo.InvariantCulture))),
            new("enc_role", _claimsProtector.Protect(user.Role?.RoleName ?? string.Empty))
        };

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new GeneratedTokenDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc
        };
    }
}
