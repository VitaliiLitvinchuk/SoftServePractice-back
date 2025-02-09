using Api.Dtos.Roles;
using Domain.Users;

namespace Api.Dtos.Users;

public record class UserDto(Guid Id, string Email, Guid RoleId, RoleDto? Role)
{
    public static UserDto FromDomainModel(User user)
        => new(user.Id.Value, user.Email, user.RoleId.Value,
            user.Role is null ? null : RoleDto.FromDomainModel(user.Role));
}
