using Domain.Users;

namespace Domain.Roles;

public class Role(RoleId id, string name)
{
    public RoleId Id { get; } = id;
    public string Name { get; private set; } = name;

    public ICollection<User> Users { get; } = [];

    public void UpdateDatails(string name)
    {
        Name = name;
    }

    public static Role New(RoleId id, string name)
        => new(id, name);
}
