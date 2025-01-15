using Domain.MoviesRatings;
using Domain.PurchaseHistories;
using Domain.Roles;

namespace Domain.Users;

public class User(UserId userId, string email, string passwordHash, RoleId roleId)
{
    public UserId Id { get; } = userId;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;

    public RoleId RoleId { get; private set; } = roleId;
    public Role? Role { get; private set; }

    public ICollection<MovieRating> MovieRatings { get; } = [];
    public ICollection<PurchaseHistory> PurchaseHistories { get; } = [];

    public void UpdateRole(RoleId roleId)
    {
        RoleId = roleId;
        Role = null;
    }

    public static User New(UserId userId, string email, string passwordHash, RoleId roleId)
        => new(userId, email, passwordHash, roleId);
}
