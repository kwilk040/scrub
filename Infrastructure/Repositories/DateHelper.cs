namespace Infrastructure.Repositories;

public static class DateHelper
{
    public static DateTime GetNowAtUserTimezone(TimeZoneInfo timeZoneInfo) =>
        TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow.ToUniversalTime(), timeZoneInfo.Id);
}