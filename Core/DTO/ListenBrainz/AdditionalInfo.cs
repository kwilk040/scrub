namespace Core.DTO.ListenBrainz;

public record AdditionalInfo(
    string SubmissionClient,
    string SubmissionClientVersion,
    long Tracknumber,
    List<string>? ArtistMbids,
    long DurationMs);