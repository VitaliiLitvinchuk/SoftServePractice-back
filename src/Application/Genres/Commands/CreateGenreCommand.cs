using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Genres.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using MediatR;

namespace Application.Genres.Commands;

public class CreateGenreCommand : IRequest<Result<Genre, GenreException>>
{
    public required string Name { get; init; }
}

public class CreateGenreCommandHandler(IBaseRepository<Genre> repository, IBaseQuery<Genre> query) : IRequestHandler<CreateGenreCommand, Result<Genre, GenreException>>
{
    public async Task<Result<Genre, GenreException>> Handle(CreateGenreCommand request, CancellationToken cancellation)
    {
        var id = GenreId.New();
        var entity = Genre.New(id, request.Name);

        var result = await query.Get(cancellation, x => x.Name == entity.Name);

        return await result.Match(
            entity => Task.FromResult<Result<Genre, GenreException>>(new GenreNameAlreadyExistsException(entity.Id, entity.Name)),
            async () => await CreateEntity(entity, cancellation)
        );
    }

    public async Task<Result<Genre, GenreException>> CreateEntity(Genre entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new GenreUnknownException(entity.Id, exception);
        }
    }
}
