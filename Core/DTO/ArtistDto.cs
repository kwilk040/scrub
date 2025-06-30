namespace Core.DTO;

public record ArtistDto
{
    public required string Name { get; init; }
    public required int Count { get; init; }
};