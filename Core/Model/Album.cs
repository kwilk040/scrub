using Core.Enums;

namespace Core.Model;

public class Album
{
    public required string Title { get; init; }
    public ISet<Artist> Artists { get; init; } = new HashSet<Artist>();
    public ISet<Track> Tracks { get; init; } = new HashSet<Track>();
    public string? CoverUrl { get; set; }
    public CoverStatus? CoverStatus { get; set; }
}