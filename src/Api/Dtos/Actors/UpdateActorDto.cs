namespace Api.Dtos.Actors;

public record UpdateActorDto(Guid Id, string Name, string Surname, string Middlename, IFormFile? Image);
