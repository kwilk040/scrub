using System.Globalization;
using Core.DTO;
using Core.Model;
using Core.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScrobbleRepository(ApplicationDbContext dbContext) : IScrobbleRepository
{
    public async Task AddAsync(Scrobble scrobble)
    {
        await dbContext.Scrobbles.AddAsync(scrobble);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Scrobble> scrobbles)
    {
        await dbContext.Scrobbles.AddRangeAsync(scrobbles);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<ScrobbleDto>> GetLastScrobblesByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo)
    {
        return await (from s in dbContext.Scrobbles
                join t in dbContext.Tracks on s.Track.Title equals t.Title
                where s.UserId == userId
                orderby s.ListenedAt descending
                select new ScrobbleDto()
                {
                    Id = s.Id,
                    Track = new TrackDto()
                    {
                        Title = t.Title,
                        Artists =
                            (from a in dbContext.Artists
                                where t.Artists.Contains(a)
                                select a.Name)
                            .ToHashSet()
                    },
                    ListenedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id),
                    CoverUrl = (from album in dbContext.Albums
                        where t.Albums.Contains(album)
                        select t.CoverUrl ?? album.CoverUrl).FirstOrDefault()
                }
            ).Take(50).ToListAsync();
    }

    public async Task<Dictionary<DayOfWeek, int>> GetWeeklyPulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo)
    {
        var now = DateHelper.GetNowAtUserTimezone(timeZoneInfo);

        var result = await (
            from s in dbContext.Scrobbles
            where s.UserId == userId
            where TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id).Date
                      .AddDays(-1 * (int)TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id)
                          .DayOfWeek) ==
                  now.Date.AddDays(-1 * (int)now.DayOfWeek)
            group s by s.ListenedAt.DayOfWeek
            into g
            select new
            {
                Day = g.Key,
                Count = g.Count()
            }).ToListAsync();

        return result.ToDictionary(r => r.Day, r => r.Count);
    }

    public async Task<Dictionary<DateOnly, int>> GetMonthlyPulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo)
    {
        var now = DateHelper.GetNowAtUserTimezone(timeZoneInfo);

        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var result = await (
            from scrobble in dbContext.Scrobbles
            where scrobble.UserId == userId
            where TimeZoneInfo.ConvertTimeBySystemTimeZoneId(scrobble.ListenedAt, timeZoneInfo.Id) >= startOfMonth &&
                  TimeZoneInfo.ConvertTimeBySystemTimeZoneId(scrobble.ListenedAt, timeZoneInfo.Id) < endOfMonth
            group scrobble by DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(scrobble.ListenedAt, timeZoneInfo.Id))
            into g
            select new
            {
                Day = g.Key,
                Count = g.Count()
            }).ToListAsync();

        return result.ToDictionary(grouping => grouping.Day, grouping => grouping.Count);
    }

    public async Task<Dictionary<string, int>> GetYearlyPulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo)
    {
        var now = DateHelper.GetNowAtUserTimezone(timeZoneInfo);

        var result = await (
            from s in dbContext.Scrobbles
            where s.UserId == userId
            where TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id).Date.Year == now.Year
            group s by TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id).Month
            into g
            select new
            {
                Month = g.Key,
                Count = g.Count()
            }).ToListAsync();

        return result.ToDictionary(grouping => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(grouping.Month),
            grouping => grouping.Count);
    }

    public async Task<Dictionary<int, int>> GetDecadePulseByUserAsync(Guid userId, TimeZoneInfo timeZoneInfo)
    {
        var now = DateHelper.GetNowAtUserTimezone(timeZoneInfo);

        var result = await (
            from s in dbContext.Scrobbles
            where s.UserId == userId
            where TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id).Date.Year >= now.Year - 10
            group s by TimeZoneInfo.ConvertTimeBySystemTimeZoneId(s.ListenedAt, timeZoneInfo.Id).Year
            into g
            select new
            {
                Year = g.Key,
                Count = g.Count()
            }).ToListAsync();

        return result.ToDictionary(grouping => grouping.Year, grouping => grouping.Count);
    }
}