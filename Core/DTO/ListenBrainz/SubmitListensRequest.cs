namespace Core.DTO.ListenBrainz;

public record SubmitListensRequest(ListenType ListenType, List<Payload> Payload);