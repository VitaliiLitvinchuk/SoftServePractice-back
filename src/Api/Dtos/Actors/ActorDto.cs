using Domain.Actors;

namespace Api.Dtos.Actors;

public record ActorDto(Guid Id, string Name, string Surname, string Middlename, string ImageUrl)
{
    public static ActorDto FromDomainModel(Actor actor)
        => new(actor.Id.Value, actor.Name, actor.Surname, actor.Middlename, actor.ImageUrl);

    public static Actor ToDomainModel(ActorDto actorDto)
        => Actor.New(new(actorDto.Id), actorDto.Name, actorDto.Surname, actorDto.Middlename, actorDto.ImageUrl);
}
