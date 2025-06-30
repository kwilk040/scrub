using Application.Services.Interfaces;
using Core.DTO;
using Core.Enums;
using Core.Repositories;

namespace Application.Services;

public class ScrobbleService(
    IScrobbleRepository scrobbleRepository,
    IArtistRepository artistRepository,
    IAlbumRepository albumRepository,
    ITrackRepository trackRepository,
    IUserSessionService userSessionService) : IScrobbleService
{
    public async Task<IReadOnlyList<TrackDto>> GetTopTracksAsync(RangeSelection range)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await trackRepository.GetTopTracksByUserAsync(
            userSessionData.UserId,
            range,
            userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyList<ArtistDto>> GetTopArtistsAsync(RangeSelection range)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await artistRepository.GetTopArtistByUser(userSessionData.UserId, range, userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyList<AlbumDto>> GetTopAlbumsAsync(RangeSelection range)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await albumRepository.GetTopAlbumsByUserAsync(userSessionData.UserId, range,
            userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyList<ScrobbleDto>> GetLastScrobblesAsync(int count = 50)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await scrobbleRepository.GetLastScrobblesByUserAsync(userSessionData.UserId,
            userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyDictionary<DayOfWeek, int>> GetWeeklyPulseAsync(int count = 50)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await scrobbleRepository.GetWeeklyPulseByUserAsync(userSessionData.UserId, userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyDictionary<DateOnly, int>> GetMonthlyPulseAsync(int count = 50)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await scrobbleRepository.GetMonthlyPulseByUserAsync(userSessionData.UserId,
            userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetYearlyPulseAsync(int count = 50)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await scrobbleRepository.GetYearlyPulseByUserAsync(userSessionData.UserId, userSessionData.TimeZoneInfo);
    }

    public async Task<IReadOnlyDictionary<int, int>> GetDecadePulseAsync(int count = 50)
    {
        var userSessionData = userSessionService.GetUserSessionData();
        return await scrobbleRepository.GetDecadePulseByUserAsync(userSessionData.UserId, userSessionData.TimeZoneInfo);
    }
}