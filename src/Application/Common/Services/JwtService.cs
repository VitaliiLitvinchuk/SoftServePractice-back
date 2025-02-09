using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Services;
using Application.Common.Settings;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Application.Common.Interfaces.Services.IJwtService;

namespace Application.Common.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly JwtSettings settings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
    public async Task<string> GenerateToken(User user, CancellationToken cancellation)
    {
        var claims = new[]
        {
            new Claim(GetClaim(ClaimsType.UserId), user.Id.ToString()),
            new Claim(GetClaim(ClaimsType.RoleId), user.RoleId.ToString()),
            new Claim(GetClaim(ClaimsType.Email), user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        var creds = new SigningCredentials(key, settings.SecurityAlgorithms);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
            signingCredentials: creds);

        return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token), cancellation);
    }

    public async Task<bool> ValidateToken(string token, CancellationToken cancellation)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
            };

            var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, tokenValidationParameters);
            if (result.SecurityToken is JwtSecurityToken jwtSecurityToken)
            {
                if (jwtSecurityToken.Header.Alg.Equals(settings.SecurityAlgorithms, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        return false;
    }
}