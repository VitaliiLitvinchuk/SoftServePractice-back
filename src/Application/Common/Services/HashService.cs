using Application.Common.Interfaces.Services;
using Application.Common.Settings;
using Microsoft.Extensions.Configuration;

namespace Application.Common.Services;

public class HashService(IConfiguration configuration) : IHashService
{
    private readonly HashSettings settings = configuration.GetSection(nameof(HashSettings)).Get<HashSettings>()!;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, settings.Salt);
    }
}
