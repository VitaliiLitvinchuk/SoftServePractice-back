using Domain.MoviesRatings;
using Domain.PurchaseHistories;
using Domain.Roles;

namespace Domain.Users;

public class User(UserId id, string email, string passwordHash, RoleId roleId)
{
    public UserId Id { get; } = id;
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

    public static User New(UserId id, string email, string passwordHash, RoleId roleId)
        => new(id, email, passwordHash, roleId);
}
