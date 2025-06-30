using Core.Enums;

namespace Core.Model;

public class Track
{
    public required string Title { get; init; }
    public required TimeSpan DurationMs { get; init; }
    public ISet<Artist> Artists { get; init; } = new HashSet<Artist>();
    public ISet<Album> Albums { get; init; } = new HashSet<Album>();
    public string? CoverUrl { get; set; }
    public CoverStatus? CoverStatus { get; set; }
}