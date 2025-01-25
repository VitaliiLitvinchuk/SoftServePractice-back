using Domain.Halls;
using Domain.Movies;
using Domain.Statuses;
using Domain.Tickets;

namespace Domain.Sessions;

public class Session(SessionId id, DateTime startAt, DateTime endAt, StatusId statusId, MovieId movieId, HallId hallId)
{
    public SessionId Id { get; } = id;

    public DateTime StartAt { get; private set; } = startAt;
    public DateTime EndAt { get; private set; } = endAt;

    public StatusId StatusId { get; private set; } = statusId;
    public MovieId MovieId { get; private set; } = movieId;
    public HallId HallId { get; private set; } = hallId;

    public Status? Status { get; private set; }
    public Movie? Movie { get; private set; }
    public Hall? Hall { get; private set; }

    public ICollection<Ticket> Tickets { get; } = [];

    public void UpdateDetails(DateTime startAt, DateTime endAt)
    {
        StartAt = startAt;
        EndAt = endAt;
    }

    public void UpdateStatus(StatusId statusId)
    {
        StatusId = statusId;
        Status = null;
    }

    public void UpdateMovie(MovieId movieId)
    {
        MovieId = movieId;
        Movie = null;
    }

    public void UpdateHall(HallId hallId)
    {
        HallId = hallId;
        Hall = null;
    }

    public static Session New(SessionId id, DateTime startAt, DateTime endAt, StatusId statusId, MovieId movieId, HallId hallId)
        => new(id, startAt, endAt, statusId, movieId, hallId);
}
