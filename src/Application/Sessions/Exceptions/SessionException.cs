using Domain.Halls;
using Domain.Movies;
using Domain.Sessions;
using Domain.Statuses;

namespace Application.Sessions.Exceptions;

public class SessionException(SessionId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public SessionId Id { get; } = id;
}

public class SessionNotFoundException(SessionId id) : SessionException(id, $"Session {id} not found.");
public class SessionHasReleationsException(SessionId id) : SessionException(id, $"Session {id} has relations.");
public class StatusForSessionNotFoundException(SessionId sessionId, StatusId statusId) : SessionException(sessionId, $"Status {statusId} for session not found.");
public class MovieForSessionNotFoundException(SessionId sessionId, MovieId movieId) : SessionException(sessionId, $"Movie {movieId} for session not found.");
public class HallForSessionNotFoundException(SessionId sessionId, HallId hallId) : SessionException(sessionId, $"Hall {hallId} for session not found.");
public class HallWillHaveSessionInThisTimeException(SessionId sessionId, HallId hallId) : SessionException(sessionId, $"Hall {hallId} will have session in this time.");
public class SessionCannotBeShorterThanMovieDurationException(SessionId id) : SessionException(id, $"Session {id} cannot be shorter than movie duration.");
public class SessionUnknownException(SessionId id, Exception innerException) : SessionException(id, $"Session {id} is unknown.", innerException);