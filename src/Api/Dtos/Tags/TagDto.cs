using Domain.Tags;

namespace Api.Dtos.Tags;

public record TagDto(Guid Id, string Name)
{
    public static TagDto FromDomainModel(Tag tag) => new(tag.Id.Value, tag.Name);
}
