using Domain.Users;

namespace Application.Common.Interfaces.Services;

public interface IJwtService
{
    Task<string> GenerateToken(User user, CancellationToken cancellation);
    Task<bool> ValidateToken(string token, CancellationToken cancellation);
}
