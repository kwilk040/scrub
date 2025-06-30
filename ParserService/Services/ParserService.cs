using System.Globalization;
using System.Text.Json;
using Core.Model;
using Core.Repositories;
using Grpc.Core;

namespace ParserService.Services;

public class ParserService(
    IArtistRepository artistRepository,
    ITrackRepository trackRepository,
    IAlbumRepository albumRepository,
    IScrobbleRepository scrobbleRepository,
    ILogger<ParserService> logger) : Parser.ParserBase
{
    private const int BatchSize = 1000;

    public override async Task<ParseSpotifyExportResponse> ParseSpotifyExport(ParseSpotifyExportRequest request,
        ServerCallContext context)
    {
        var json = request.Json;
        var userId = request.UserId;
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var jsonDocument = JsonDocument.Parse(json);
        var dataElement = jsonDocument.RootElement.GetProperty("data");

        var scrobbles = await Parse(dataElement, jsonOptions, Guid.Parse(userId));

        return await BatchInsertScrobbles(scrobbles);
    }

    private async Task<List<Scrobble>> MapSpotifyScrobbleToScrobbleAsync(List<SpotifyScrobble>? spotifyScrobbles,
        Guid userId)
    {
        List<Scrobble> scrobbles = [];
        if (spotifyScrobbles == null) return scrobbles;
        foreach (var (title, artistName, releaseName, msPlayed, ts, _, platform) in spotifyScrobbles)
        {
            var artist = await artistRepository
                .GetByNameAsync(artistName);

            if (artist == null)
            {
                artist = new Artist
                    { Name = artistName };
                await artistRepository.AddAsync(artist);
            }


            var album = await albumRepository.GetByNameAsync(releaseName);

            if (album == null)
            {
                album = new Album
                {
                    Title = releaseName,
                };

                album.Artists.Add(artist);
                await albumRepository.AddAsync(album);
            }

            album.Artists.Add(artist);

            var track = await trackRepository.GetByTitleAsync(title);

            if (track == null)
            {
                track = new Track
                {
                    Title = title,
                    Artists = new HashSet<Artist>() { artist },
                    Albums = new HashSet<Album>() { album },
                    DurationMs = TimeSpan.FromMilliseconds(msPlayed),
                };
                await trackRepository.AddAsync(track);
            }

            track.Artists.Add(artist);
            track.Albums.Add(album);

            var scrobble = new Scrobble
            {
                Id = Guid.NewGuid(),
                Track = track,
                UserId = userId,
                ListenedAt = DateTime.Parse(ts, null,
                    DateTimeStyles.AssumeUniversal).ToUniversalTime(),
                SubmissionClient = "Spotify",
                SubmissionClientVersion = platform,
            };
            scrobbles.Add(scrobble);
        }

        return scrobbles;
    }

    private async Task<List<Scrobble>> Parse(JsonElement scrobbles, JsonSerializerOptions jsonOptions, Guid userId)
    {
        var spotifyScrobbles = scrobbles.Deserialize<List<SpotifyScrobble>>(jsonOptions)
            ?.Where(x => x.Skipped == false)
            .Where(x => x.IsValid())
            .ToList();

        return await MapSpotifyScrobbleToScrobbleAsync(spotifyScrobbles, userId);
    }

    private async Task<ParseSpotifyExportResponse> BatchInsertScrobbles(List<Scrobble> scrobbles)
    {
        var count = scrobbles.Count;
        var insertedCount = 0;
        var scrobbleBatches = BatchList(scrobbles, BatchSize);
        foreach (var scrobbleBatch in scrobbleBatches)
        {
            try
            {
                await scrobbleRepository.AddRangeAsync(scrobbleBatch);
                insertedCount += scrobbleBatch.Count;
            }
            catch (Exception e)
            {
                logger.LogWarning("Failed to insert batch\n{EMessage}", e.Message);
            }
        }

        return new ParseSpotifyExportResponse()
        {
            Status = (insertedCount == 0 && count > 0) ? Status.Error
                : (insertedCount < count) ? Status.PartialSuccess
                : Status.Success,
            ScrobbleCount = insertedCount,
            Message = $"Inserted {insertedCount} records"
        };
    }

    private static IEnumerable<List<T>> BatchList<T>(List<T> source, int batchSize)
    {
        for (var i = 0; i < source.Count; i += batchSize)
        {
            yield return source.Skip(i).Take(batchSize).ToList();
        }
    }
}