using Domain.Roles;

namespace Api.Dtos.Roles;

public record RoleDto(Guid Id, string Name)
{
    public static RoleDto FromDomainModel(Role role)
        => new(role.Id.Value, role.Name);

    public static Role ToDomainModel(RoleDto dto)
        => Role.New(new(dto.Id), dto.Name);
}
