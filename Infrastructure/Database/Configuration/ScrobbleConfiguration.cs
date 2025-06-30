using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class ScrobbleConfiguration : IEntityTypeConfiguration<Scrobble>
{
    public void Configure(EntityTypeBuilder<Scrobble> builder)
    {
        builder.HasKey(scrobble => scrobble.Id);
        builder.HasOne(scrobble => scrobble.Track);
        builder.HasIndex(scrobble => scrobble.ListenedAt);
    }
}