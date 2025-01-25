using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.GenresTags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Domain.GenresTags;
using Domain.Tags;
using MediatR;

namespace Application.GenresTags.Commands;

public class CreateGenreTagCommand : IRequest<Result<GenreTag, GenreTagException>>
{
    public required Guid GenreId { get; init; }
    public required Guid TagId { get; init; }
}

public class CreateGenreTagCommandHandler(IBaseRepository<GenreTag> repository, IBaseQuery<GenreTag> query, IBaseQuery<Genre> genreQuery, IBaseQuery<Tag> tagQuery) : IRequestHandler<CreateGenreTagCommand, Result<GenreTag, GenreTagException>>
{
    public async Task<Result<GenreTag, GenreTagException>> Handle(CreateGenreTagCommand request, CancellationToken cancellation)
    {
        var genreId = GenreId.New();
        var tagId = TagId.New();

        var entity = GenreTag.New(genreId, tagId);

        var result = await query.Get(cancellation, x => x.GenreId == entity.GenreId && x.TagId == entity.TagId);

        return await result.Match(
            entity => Task.FromResult<Result<GenreTag, GenreTagException>>(new GenreTagAlreadyExistsException(entity.GenreId, entity.TagId)),
            async () =>
            {
                var genre = await genreQuery.Get(cancellation, x => x.Id == entity.GenreId);

                return await genre.Match(
                    async genre =>
                    {
                        var tag = await tagQuery.Get(cancellation, x => x.Id == entity.TagId);

                        return await tag.Match(
                            async tag => await CreateEntity(entity, cancellation),
                            () => Task.FromResult<Result<GenreTag, GenreTagException>>(new TagForGenreTagNotFoundException(entity.TagId))
                        );
                    },
                    () => Task.FromResult<Result<GenreTag, GenreTagException>>(new GenreForGenreTagNotFoundException(entity.GenreId))
                );
            }
        );
    }

    private async Task<Result<GenreTag, GenreTagException>> CreateEntity(GenreTag entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new GenreTagUnknownException(entity.GenreId, entity.TagId, exception);
        }
    }
}
