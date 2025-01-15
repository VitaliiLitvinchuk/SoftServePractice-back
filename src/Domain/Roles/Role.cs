namespace Domain.Roles;

public class Role(RoleId roleId, string name)
{
    public RoleId Id { get; } = roleId;
    public string Name { get; private set; } = name;

    public void UpdateDatails(string name)
    {
        Name = name;
    }

    public static Role New(RoleId roleId, string name)
        => new(roleId, name);
}
