using Core.DTO;

namespace Application.Services.Interfaces;

public interface IApiKeyService
{
    public Task<ApiKeyDto> GenerateApiKeyAsync(string name);
    public Task<IReadOnlyCollection<ApiKeyDto>> GetApiKeysForUserAsync();
    public Task DeleteApiKeyAsync(string apiKey);
}