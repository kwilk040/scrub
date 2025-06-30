namespace Core.DTO;

public class ScrobbleDto
{
    public required Guid Id { get; init; }
    public required TrackDto Track { get; init; }
    public required DateTime ListenedAt { get; init; }
    public string? CoverUrl { get; init; }
}