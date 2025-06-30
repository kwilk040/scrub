using Core.Model;

namespace Core.DTO;

public class ApiKeyDto
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }

    public static ApiKeyDto FromApiKey(ApiKey key, TimeZoneInfo timeZoneInfo)
    {
        return new ApiKeyDto()
        {
            Key = key.Key,
            Name = key.Name,
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(key.CreatedAt, timeZoneInfo.Id),
        };
    }
}