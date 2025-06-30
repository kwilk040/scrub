using Core.DTO;
using Core.Enums;
using Core.Model;
using Core.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ArtistRepository(ApplicationDbContext dbContext) : IArtistRepository
{
    public async Task<Artist?> GetByNameAsync(string name)
    {
        return await dbContext.Artists.FirstOrDefaultAsync(artist => artist.Name == name);
    }

    public async Task<List<ArtistDto>> GetTopArtistByUser(Guid userId, RangeSelection range, TimeZoneInfo timeZoneInfo)
    {
        var nowAtUserTimezone =
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow.ToUniversalTime(), timeZoneInfo.Id);

        var baseQuery = from scrobble in dbContext.Scrobbles
            join track in dbContext.Tracks on scrobble.Track.Title equals track.Title
            from artist in dbContext.Artists
            where scrobble.UserId == userId
            where track.Artists.Contains(artist)
            select new
            {
                scrobble, artist,
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
            group st.artist by st.artist.Name
            into g
            orderby g.Count() descending, g.Key descending
            select new ArtistDto()
            {
                Name = g.Key,
                Count = g.Count()
            }).Take(30).ToListAsync();
    }

    public async Task AddAsync(Artist artist)
    {
        await dbContext.Artists.AddAsync(artist);
        await dbContext.SaveChangesAsync();
    }
}