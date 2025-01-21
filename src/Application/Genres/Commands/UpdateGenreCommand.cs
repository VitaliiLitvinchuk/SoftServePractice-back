using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Genres.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using MediatR;

namespace Application.Genres.Commands;

public class UpdateGenreCommand : IRequest<Result<Genre, GenreException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateGenreCommandHandler(IBaseRepository<Genre> repository, IBaseQuery<Genre> query) : IRequestHandler<UpdateGenreCommand, Result<Genre, GenreException>>
{
    public async Task<Result<Genre, GenreException>> Handle(UpdateGenreCommand request, CancellationToken cancellation)
    {
        var id = new GenreId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async genre =>
            {
                var result = await query.Get(cancellation, x => x.Name == request.Name);

                return await result.Match(
                  entity => Task.FromResult<Result<Genre, GenreException>>(new GenreNameAlreadyExistsException(entity.Id, entity.Name)),
                  async () => await UpdateEntity(genre, request.Name, cancellation)
                );
            },
            () => Task.FromResult<Result<Genre, GenreException>>(new GenreNotFoundException(id))
        );
    }

    private async Task<Result<Genre, GenreException>> UpdateEntity(Genre entity, string name, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDatails(name);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new GenreUnknownException(entity.Id, exception);
        }
    }
}
