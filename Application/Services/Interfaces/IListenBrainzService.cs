using Core.DTO.ListenBrainz;

namespace Application.Services.Interfaces;

public interface IListenBrainzService
{
    Task<ValidateTokenResponse> ValidateToken(string token);

    Task<SubmitListensResponse> SubmitListens(SubmitListensRequest submitListensRequest, string token);
}