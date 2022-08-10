namespace ZeeDash.API.GraphQLServer.Models;

public record class Human(
    Guid Id,
    string Name,
    string HomePlanet,
    DateOnly DateOfBirth,
    DateTimeOffset Created,
    DateTimeOffset Modified) :
    Character(Id, Name, Created, Modified)
{
}
