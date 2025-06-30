using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Scrobble> Scrobbles { get; set; }

    public DbSet<ApiKey> ApiKeys { get; set; }

    public DbSet<InviteCode> InviteCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.HasKey(x => x.UserId); });
        modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.HasKey(x => x.Id); });
        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        });
        modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.HasKey(x => x.Id); });
        modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.HasKey(x => new { x.UserId, x.RoleId }); });
        modelBuilder.UseIdentityByDefaultColumns();
        
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}