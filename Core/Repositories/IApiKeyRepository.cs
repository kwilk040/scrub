using Core.DTO;
using Core.Model;

namespace Core.Repositories;

public interface IApiKeyRepository
{
    Task AddAsync(ApiKey apiKey);

    Task<ApiKey?> GetApiKeyAsync(string key);
    Task<IReadOnlyCollection<ApiKeyDto>> GetApiKeysForUserAsync(string userId, TimeZoneInfo timeZoneInfo);
    Task DeleteApiKeyAsync(string apiKey, string userId);
}