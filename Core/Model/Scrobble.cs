namespace Core.Model;

public class Scrobble
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Track Track { get; init; }
    public required DateTime ListenedAt { get; init; }
    public required string SubmissionClient { get; init; }
    public required string SubmissionClientVersion { get; init; }
}