namespace Core.Model;

public class ApiKey
{
    public required Guid Id { get; init; }


    public required string Key { get; init; }

    public required string Name { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required string UserId { get; init; }

    public ApplicationUser User { get; init; }
}