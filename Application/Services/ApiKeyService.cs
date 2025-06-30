using System.Security.Cryptography;
using Application.Services.Interfaces;
using Core.DTO;
using Core.Model;
using Core.Repositories;

namespace Application.Services;

public class ApiKeyService(IApiKeyRepository apiKeyRepository, IUserSessionService userSessionService) : IApiKeyService
{
    private const int KeyLength = 32;
    private const string Prefix = "SCRB-";

    public async Task<ApiKeyDto> GenerateApiKeyAsync(string name)
    {
        var bytes = RandomNumberGenerator.GetBytes(KeyLength);
        var base64String = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
        var keyLengthWithoutPrefix = KeyLength - Prefix.Length;
        var key = Prefix + base64String[..keyLengthWithoutPrefix];

        var apiKey = new ApiKey()
        {
            Id = Guid.NewGuid(),
            Key = key,
            Name = name,
            UserId = userSessionService.GetUserSessionData().UserId.ToString(),
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
        };

        await apiKeyRepository.AddAsync(apiKey);

        return ApiKeyDto.FromApiKey(apiKey, userSessionService.GetUserSessionData().TimeZoneInfo);
    }

    public async Task<IReadOnlyCollection<ApiKeyDto>> GetApiKeysForUserAsync()
    {
        return await apiKeyRepository.GetApiKeysForUserAsync(userSessionService.GetUserSessionData().UserId.ToString(),
            userSessionService.GetUserSessionData().TimeZoneInfo);
    }

    public async Task DeleteApiKeyAsync(string apiKey)
    {
        await apiKeyRepository.DeleteApiKeyAsync(apiKey,
            userSessionService.GetUserSessionData().UserId.ToString());
    }
}