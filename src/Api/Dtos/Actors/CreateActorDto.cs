namespace Api.Dtos.Actors;

public record CreateActorDto(string Name, string Surname, string Middlename, IFormFile Image);
