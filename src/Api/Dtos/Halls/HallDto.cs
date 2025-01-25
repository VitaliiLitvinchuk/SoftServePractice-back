using Domain.Halls;

namespace Api.Dtos.Halls;

public record HallDto(Guid Id, string Name, short Capacity)
{
    public static HallDto FromDomainModel(Hall hall) => new(hall.Id.Value, hall.Name, hall.Capacity);
}
