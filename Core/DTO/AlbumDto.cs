namespace Core.DTO;

public record AlbumDto
{
    public required string Title { get; init; }
    public required int Count { get; init; }
    public required HashSet<string> Artists { get; init; }
    public string? CoverUrl { get; init; }
}