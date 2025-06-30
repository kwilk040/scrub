using Core.DTO;
using Core.Enums;
using Core.Model;
using Core.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TrackRepository(ApplicationDbContext dbContext) : ITrackRepository
{
    public async Task AddAsync(Track track)
    {
        await dbContext.Tracks.AddAsync(track);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Track?> GetByTitleAsync(string title)
    {
        return await dbContext.Tracks
            .Include(track => track.Artists)
            .Include(track => track.Albums)
            .FirstOrDefaultAsync(track => track.Title == title);
    }

    public async Task<List<TrackDto>> GetTopTracksByUserAsync(Guid userId, RangeSelection range,
        TimeZoneInfo timeZoneInfo)
    {
        var nowAtUserTimezone = DateHelper.GetNowAtUserTimezone(timeZoneInfo);

        var baseQuery = from scrobble in dbContext.Scrobbles
            join track in dbContext.Tracks on scrobble.Track.Title equals track.Title
            where scrobble.UserId == userId
            select new ScrobbleWithTrackInfo()
            {
                Scrobble = scrobble,
                Track = track,
                ListenedAtUserTz = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(scrobble.ListenedAt, timeZoneInfo.Id)
            };

        var query = range switch
        {
            RangeSelection.Today =>
                from st in baseQuery
                where st.ListenedAtUserTz.Date ==
                      nowAtUserTimezone.Date
                select st,
            RangeSelection.ThisWeek =>
                from st in baseQuery
                where st.ListenedAtUserTz.Date
                          .AddDays(-1 * (int)st.ListenedAtUserTz.DayOfWeek) ==
                      nowAtUserTimezone.Date.AddDays(-1 * (int)nowAtUserTimezone.DayOfWeek)
                select st,
            RangeSelection.ThisMonth =>
                from st in baseQuery
                where st.ListenedAtUserTz.Month == nowAtUserTimezone.Month
                where st.ListenedAtUserTz.Year == nowAtUserTimezone.Year
                select st,
            RangeSelection.ThisYear =>
                from st in baseQuery
                where st.ListenedAtUserTz.Year == nowAtUserTimezone.Year
                select st,
            RangeSelection.AllTime =>
                baseQuery,
        };

        return await (from st in query
            group st.Track by st.Track.Title
            into grouped
            orderby grouped.Count() descending, grouped.Key descending
            select new TrackDto()
            {
                Title = grouped.Key,
                Count = grouped.Count(),
                Artists =
                    (from t in grouped from a in dbContext.Artists where t.Artists.Contains(a) select a.Name)
                    .ToHashSet(),
                CoverUrl = (from t in grouped
                        from a in dbContext.Albums
                        where t.Albums.Contains(a)
                        select t.CoverUrl ?? a.CoverUrl)
                    .FirstOrDefault(),
            }).Take(30).ToListAsync();
    }
}