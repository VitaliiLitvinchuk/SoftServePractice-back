using Api.Dtos.Actors;
using Api.Dtos.Movies;
using Domain.MoviesActors;

namespace Api.Dtos.MoviesActors;

public record MovieActorDto(Guid MovieId, Guid ActorId, MovieDto? Movie, ActorDto? Actor)
{
    public static MovieActorDto FromDomainModel(MovieActor movieActor)
        => new(movieActor.MovieId.Value, movieActor.ActorId.Value,
            movieActor.Movie is null ? null : MovieDto.FromDomainModel(movieActor.Movie),
            movieActor.Actor is null ? null : ActorDto.FromDomainModel(movieActor.Actor));
}
