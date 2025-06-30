namespace Core.DTO.ListenBrainz;

public class ValidateTokenResponse
{
    public required int Code { get; init; }
    public required string Message { get; init; }
    public required bool Valid { get; init; }
    public string? UserName { get; init; }
}