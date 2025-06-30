using Core.DTO;
using Core.Enums;
using Core.Model;

namespace Core.Repositories;

public interface IAlbumRepository
{
    public Task<Album?> GetByNameAsync(string name);
    Task<List<AlbumDto>> GetTopAlbumsByUserAsync(Guid userId, RangeSelection range, TimeZoneInfo timeZoneInfo);
    Task AddAsync(Album value);
    public Task<Album?> GetRecordForCoverFetcherAsync();
    public Task Update(Album album);
}