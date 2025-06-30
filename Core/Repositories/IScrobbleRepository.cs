using Core.DTO;
using Core.Model;

namespace Core.Repositories;

public interface IScrobbleRepository
{
    Task AddAsync(Scrobble scrobble);
    Task AddRangeAsync(IEnumerable<Scrobble> scrobbles);
    Task<List<ScrobbleDto>> GetLastScrobblesByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo);
    Task<Dictionary<DayOfWeek, int>> GetWeeklyPulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo);
    Task<Dictionary<DateOnly, int>> GetMonthlyPulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo);
    Task<Dictionary<string, int>> GetYearlyPulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo);
    Task<Dictionary<int, int>> GetDecadePulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo);
}