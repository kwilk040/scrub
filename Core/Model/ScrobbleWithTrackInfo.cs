namespace Core.Model;

public class ScrobbleWithTrackInfo()
{
    public Scrobble Scrobble { get; init; }
    public Track Track { get; init; }
    public DateTime ListenedAtUserTz { get; init; }
}