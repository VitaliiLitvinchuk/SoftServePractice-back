using Domain.Halls;
using Domain.Movies;
using Domain.Statuses;
using Domain.Tickets;

namespace Domain.Sessions;

public class Session(SessionId sessionId, DateTime startAt, DateTime endAt, StatusId statusId, MovieId movieId, HallId hallId)
{
    public SessionId SessionId { get; } = sessionId;

    public DateTime StartAt { get; private set; } = startAt;
    public DateTime EndAt { get; private set; } = endAt;

    public StatusId StatusId { get; private set; } = statusId;
    public MovieId MovieId { get; private set; } = movieId;
    public HallId HallId { get; private set; } = hallId;

    public Status? Status { get; private set; }
    public Movie? Movie { get; private set; }
    public Hall? Hall { get; private set; }

    public ICollection<Ticket> Tickets { get; } = [];

    public void UpdateDatails(DateTime startAt, DateTime endAt)
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

    public static Session New(SessionId sessionId, DateTime startAt, DateTime endAt, StatusId statusId, MovieId movieId, HallId hallId)
        => new(sessionId, startAt, endAt, statusId, movieId, hallId);
}
