using Api.Dtos.Halls;
using Domain.Seats;

namespace Api.Dtos.Seats;

public record SeatDto(Guid Id, int Row, int Number, Guid HallId, HallDto? Hall)
{
    public static SeatDto FromDomainModel(Seat seat)
        => new(seat.Id.Value, seat.Row, seat.Number, seat.HallId.Value,
            seat.Hall == null ? null : HallDto.FromDomainModel(seat.Hall));
}
