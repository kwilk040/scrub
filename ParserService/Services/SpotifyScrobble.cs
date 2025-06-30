namespace ParserService.Services;

public record SpotifyScrobble(
    string MasterMetadataTrackName,
    string MasterMetadataAlbumArtistName,
    string MasterMetadataAlbumAlbumName,
    long MsPlayed,
    string Ts,
    bool Skipped,
    string Platform)
{
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(MasterMetadataTrackName) && !string.IsNullOrEmpty(MasterMetadataAlbumArtistName) &&
               !string.IsNullOrEmpty(MasterMetadataAlbumAlbumName) && !string.IsNullOrEmpty(Ts);
    }
};