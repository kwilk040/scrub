using Microsoft.AspNetCore.Identity;

namespace Core.Model;

public class ApplicationUser : IdentityUser
{
    public ICollection<ApiKey> ApiKeys { get; set; }
}