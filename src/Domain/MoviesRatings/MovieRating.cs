using Domain.Movies;
using Domain.Users;

namespace Domain.MoviesRatings;

public class MovieRating(UserId userId, MovieId movieId, int rate)
{
    public UserId UserId { get; } = userId;
    public MovieId MovieId { get; } = movieId;

    public int Rate { get; private set; } = rate;

    public void UpdateDetails(int rate)
    {
        Rate = rate;
    }

    public static MovieRating New(UserId userId, MovieId movieId, int rate) => new(userId, movieId, rate);
}
