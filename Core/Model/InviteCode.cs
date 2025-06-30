namespace Core.Model;

public class InviteCode
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; }
    public string? UsedByUserId { get; set; }
    public ApplicationUser? UsedByUser { get; set; }
    public DateTime? Expiration { get; set; }
}