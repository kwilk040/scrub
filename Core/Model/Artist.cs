using Core.Enums;

namespace Core.Model;

public class Artist
{
    public string Name { get; init; }
    public ISet<Album> Albums { get; init; } = new HashSet<Album>();
    public ISet<Track> Tracks { get; init; } = new HashSet<Track>();
    public string? CoverUrl { get; set; }
    public CoverStatus? CoverStatus { get; set; }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}