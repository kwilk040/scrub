namespace Application;

public class UserSessionData
{
    public required TimeZoneInfo TimeZoneInfo { get; init; }
    public required Guid UserId { get; init; }
}