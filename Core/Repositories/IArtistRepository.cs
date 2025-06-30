using Core.DTO;
using Core.Enums;
using Core.Model;

namespace Core.Repositories;

public interface IArtistRepository
{
    public Task<Artist?> GetByNameAsync(string name);
    public Task<List<ArtistDto>> GetTopArtistByUser(Guid userId, RangeSelection range, TimeZoneInfo timeZoneInfo);
    Task AddAsync(Artist artist);
}