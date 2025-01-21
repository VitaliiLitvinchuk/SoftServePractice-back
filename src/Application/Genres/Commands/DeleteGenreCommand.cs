using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Genres.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Genres.Commands;

public class DeleteGenreCommand : IRequest<Result<Genre, GenreException>>
{
    public required Guid Id { get; init; }
}

public class DeleteGenreCommandHandler(IBaseRepository<Genre> repository, IBaseQuery<Genre> query) : IRequestHandler<DeleteGenreCommand, Result<Genre, GenreException>>
{
    public async Task<Result<Genre, GenreException>> Handle(DeleteGenreCommand request, CancellationToken cancellation)
    {
        var id = new GenreId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Movies).Include(x => x.Tags));

        return await result.Match(
            async genre =>
            {
                if (genre.Movies.Count != 0)
                    return new GenreHasReleationsException(id);

                if (genre.Tags.Count != 0)
                    return new GenreHasReleationsException(id);

                return await DeleteEntity(genre, cancellation);
            },
            () => Task.FromResult<Result<Genre, GenreException>>(new GenreNotFoundException(id))
        );
    }

    public async Task<Result<Genre, GenreException>> DeleteEntity(Genre entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new GenreUnknownException(entity.Id, exception);
        }
    }
}
