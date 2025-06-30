using Core.DTO;
using Core.Enums;

namespace Application.Services.Interfaces;

public interface IScrobbleService
{
    Task<IReadOnlyList<TrackDto>> GetTopTracksAsync(RangeSelection range);
    Task<IReadOnlyList<ArtistDto>> GetTopArtistsAsync(RangeSelection range);
    Task<IReadOnlyList<AlbumDto>> GetTopAlbumsAsync(RangeSelection range);
    Task<IReadOnlyList<ScrobbleDto>> GetLastScrobblesAsync(int count = 50);
    Task<IReadOnlyDictionary<DayOfWeek, int>> GetWeeklyPulseAsync(int count = 50);
    Task<IReadOnlyDictionary<DateOnly, int>> GetMonthlyPulseAsync(int count = 50);
    Task<IReadOnlyDictionary<string, int>> GetYearlyPulseAsync(int count = 50);
    Task<IReadOnlyDictionary<int, int>> GetDecadePulseAsync(int count = 50);
}
