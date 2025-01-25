using Domain.Statuses;

namespace Api.Dtos.Statuses;

public record StatusDto(Guid Id, string Name)
{
    public static StatusDto FromDomainModel(Status status) => new(status.Id.Value, status.Name);
}
