using Core.DTO;
using Core.Enums;
using Core.Model;
using Core.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlbumRepository(ApplicationDbContext dbContext) : IAlbumRepository
{
    public async Task<Album?> GetByNameAsync(string name)
    {
        return await dbContext.Albums.Include(album => album.Artists).FirstOrDefaultAsync(album => album.Title == name);
    }

    public async Task<List<AlbumDto>> GetTopAlbumsByUserAsync(Guid userId, RangeSelection range,
        TimeZoneInfo timeZoneInfo)
    {
        var nowAtUserTimezone =
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow.ToUniversalTime(), timeZoneInfo.Id);

        var baseQuery = from scrobble in dbContext.Scrobbles
            join track in dbContext.Tracks on scrobble.Track.Title equals track.Title
            from album in dbContext.Albums
            where scrobble.UserId == userId
            where track.Albums.Contains(album)
            select new
            {
                scrobble, track, album,
                ListenedAtUserTZ = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(scrobble.ListenedAt, timeZoneInfo.Id),
            };

        baseQuery = range switch
        {
            RangeSelection.Today =>
                from st in baseQuery
                where st.ListenedAtUserTZ.Date ==
                      nowAtUserTimezone.Date
                select st,
            RangeSelection.ThisWeek =>
                from st in baseQuery
                where st.ListenedAtUserTZ.Date
                          .AddDays(-1 * (int)st.ListenedAtUserTZ.DayOfWeek) ==
                      nowAtUserTimezone.Date.AddDays(-1 * (int)nowAtUserTimezone.DayOfWeek)
                select st,
            RangeSelection.ThisMonth =>
                from st in baseQuery
                where st.ListenedAtUserTZ.Month == nowAtUserTimezone.Month
                where st.ListenedAtUserTZ.Year == nowAtUserTimezone.Year
                select st,
            RangeSelection.ThisYear =>
                from st in baseQuery
                where st.ListenedAtUserTZ.Year == nowAtUserTimezone.Year
                select st,
            RangeSelection.AllTime =>
                baseQuery,
        };

        return await (from st in baseQuery
                group st.album by st.album.Title
                into g
                orderby g.Count() descending, g.Key descending
                select new AlbumDto()
                {
                    Title = g.Key,
                    Count = g.Count(),
                    Artists = (from a in g
                        from artist in dbContext.Artists
                        where a.Artists.Contains(artist)
                        select artist.Name).ToHashSet(),
                    CoverUrl = (from a in g select a.CoverUrl).FirstOrDefault()
                }
            ).Take(30).ToListAsync();
    }

    public async Task AddAsync(Album value)
    {
        await dbContext.Albums.AddAsync(value);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Album?> GetRecordForCoverFetcherAsync()
    {
        return await dbContext.Albums.Include(x => x.Artists)
            .Where(album => album.CoverStatus == null).Take(1)
            .FirstOrDefaultAsync();
    }

    public async Task Update(Album album)
    {
        dbContext.Albums.Update(album);
        await dbContext.SaveChangesAsync();
    }
}