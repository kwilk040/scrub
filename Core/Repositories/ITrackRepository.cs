using Core.DTO;
using Core.Enums;
using Core.Model;

namespace Core.Repositories;

public interface ITrackRepository
{
    Task AddAsync(Track track);
    Task<Track?> GetByTitleAsync(string title);
    Task<List<TrackDto>> GetTopTracksByUserAsync(Guid userId, RangeSelection range, TimeZoneInfo timeZoneInfo);
}