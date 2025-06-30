using Core.DTO;
using Core.Model;
using Core.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ApiKeyRepository(ApplicationDbContext dbContext) : IApiKeyRepository
{
    public async Task AddAsync(ApiKey apiKey)
    {
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ApiKey?> GetApiKeyAsync(string key)
    {
        return await dbContext.ApiKeys.Where(apiKey => apiKey.Key == key).Include(x => x.User).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<ApiKeyDto>> GetApiKeysForUserAsync(string userId, TimeZoneInfo timeZoneInfo)
    {
        return await dbContext.ApiKeys.Where(apiKey => apiKey.UserId == userId)
            .Select(apiKey => ApiKeyDto.FromApiKey(apiKey, timeZoneInfo)).ToListAsync();
    }

    public async Task DeleteApiKeyAsync(string apiKey, string userId)
    {
        dbContext.Remove(await dbContext.ApiKeys.SingleAsync(key => key.UserId == userId && key.Key == apiKey));
        await dbContext.SaveChangesAsync();
    }
}