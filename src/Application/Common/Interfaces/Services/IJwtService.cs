using Domain.Users;

namespace Application.Common.Interfaces.Services;

public interface IJwtService
{
    Task<string> GenerateToken(User user, CancellationToken cancellation);
    Task<bool> ValidateToken(string token, CancellationToken cancellation);

    enum ClaimsType { UserId = 0, RoleId = 1, Email };

    private static readonly Dictionary<ClaimsType, string> Claims = new()
    {
        { ClaimsType.UserId, "userId" },
        { ClaimsType.RoleId, "roleId" },
        { ClaimsType.Email, "email" },
    };

    public static string GetClaim(ClaimsType type)
    {
        return Claims[type];
    }
}
