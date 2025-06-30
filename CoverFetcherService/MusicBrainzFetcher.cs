using System.Text.Json;
using System.Web;

namespace CoverFetcherService;

public class MusicBrainzFetcher(ILogger<MusicBrainzFetcher> logger, HttpClient httpClient)
{
    private static readonly Uri MusicbrainzBaseUrl = new("https://musicbrainz.org/ws/2/");
    private static readonly Uri CoverArtArchiveBaseUrl = new("https://coverartarchive.org/");
    private static readonly string[] ThumbnailSizeOrder = ["500", "large", "1200", "250", "small"];


    public async Task<string?> FetchAlbumCoverAsync(HashSet<string> artists, string albumTitle)
    {
        try
        {
            var mbid = await FetchAlbumMbidAsync(artists, albumTitle);
            if (mbid is not null) return await FetchAlbumArtAsync(mbid);
            logger.LogWarning("Could not find mbid for {AlbumTitle}", albumTitle);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return null;
        }
    }

    private async Task<string?> FetchAlbumMbidAsync(HashSet<string> artists, string albumTitle)
    {
        var search = string.Concat($"release:\"{albumTitle}\"",
            string.Join("", artists.Select(artist => $" artist:\"{artist}\"").ToHashSet()));
        var query = HttpUtility.UrlEncode(search);

        var url = $"{MusicbrainzBaseUrl}release?fmt=json&query={query}";
        var response = await httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json);
        if (data.RootElement.TryGetProperty("releases", out var releases) &&
            releases.ValueKind == JsonValueKind.Array && releases.GetArrayLength() > 0)
        {
            return releases[0].GetProperty("id").GetString();
        }

        return null;
    }

    private async Task<string?> FetchAlbumArtAsync(string mbid)
    {
        var url = $"{CoverArtArchiveBaseUrl}release/{mbid}?fmt=json";
        var response = await httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json);

        var thumbnails = data.RootElement.GetProperty("images")[0].GetProperty("thumbnails");
        return ThumbnailSizeOrder
            .Select(size => thumbnails.TryGetProperty(size, out var thumbnail) ? thumbnail.GetString() : null)
            .FirstOrDefault(imageUrl => imageUrl is not null)
            ?.Replace("http://", "https://");
    }
}