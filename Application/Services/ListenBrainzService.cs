using Application.Services.Interfaces;
using Core.DTO.ListenBrainz;
using Core.Model;
using Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ListenBrainzService(
    IScrobbleRepository scrobbleRepository,
    IArtistRepository artistRepository,
    ITrackRepository trackRepository,
    IAlbumRepository albumRepository,
    ILogger<ListenBrainzService> logger,
    IApiKeyRepository apiKeyRepository
) : IListenBrainzService
{
    public async Task<ValidateTokenResponse> ValidateToken(string token)
    {
        var apiKey = await apiKeyRepository.GetApiKeyAsync(token);
        if (apiKey == null)
        {
            return new ValidateTokenResponse
            {
                Code = 200,
                Message = "Token invalid.",
                Valid = false
            };
        }


        return new ValidateTokenResponse
        {
            Code = 200,
            Message = "Token valid.",
            Valid = true,
            UserName = apiKey.User.UserName
        };
    }

    public async Task<SubmitListensResponse> SubmitListens(SubmitListensRequest submitListensRequest, string token)
    {
        if (submitListensRequest.ListenType == ListenType.PlayingNow)
        {
            return new SubmitListensResponse("ok");
        }

        var apiKey = await apiKeyRepository.GetApiKeyAsync(token);


        var payload = submitListensRequest.Payload.FirstOrDefault() ?? throw new ArgumentException("Payload was empty");
        logger.LogInformation("Payload: {Payload}", payload);

        var artists = await FetchArtist(payload.TrackMetadata.ArtistName);

        var releaseName = payload.TrackMetadata.ReleaseName;
        var album = await albumRepository.GetByNameAsync(releaseName) ?? new Album
        {
            Title = releaseName,
        };

        album.Artists.UnionWith(artists);


        var trackName = payload.TrackMetadata.TrackName;
        var track = await trackRepository.GetByTitleAsync(trackName) ?? new Track
        {
            Title = trackName,
            Artists = artists,
            Albums = new HashSet<Album>() { album },
            DurationMs = TimeSpan.FromMilliseconds(payload.TrackMetadata.AdditionalInfo.DurationMs),
        };

        track.Artists.UnionWith(artists);
        track.Albums.UnionWith(new HashSet<Album>() { album });

        var scrobble = new Scrobble
        {
            Id = Guid.NewGuid(),
            Track = track,
            UserId = Guid.Parse(apiKey.UserId),
            ListenedAt = DateTimeOffset.FromUnixTimeSeconds(payload.ListenedAt).UtcDateTime,
            SubmissionClient = payload.TrackMetadata.AdditionalInfo.SubmissionClient,
            SubmissionClientVersion = payload.TrackMetadata.AdditionalInfo.SubmissionClientVersion,
        };

        await scrobbleRepository.AddAsync(scrobble);

        return new SubmitListensResponse("ok");
    }


    private async Task<ISet<Artist>> FetchArtist(string artistName)
    {
        var artistNames = artistName.Split('\u2022')
            .Select(name => name.Trim());

        var artists = new HashSet<Artist>();

        foreach (var name in artistNames)
        {
            artists.Add(await artistRepository.GetByNameAsync(name) ?? new Artist { Name = name });
        }

        return artists;
    }
}