namespace Core.DTO;

public class TrackDto
{
    public required string Title { get; init; }
    public int Count { get; init; }
    public required HashSet<string> Artists { get; init; }
    public string? CoverUrl { get; init; }
}