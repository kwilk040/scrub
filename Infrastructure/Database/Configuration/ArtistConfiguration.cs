using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(artist => artist.Name);
        builder.HasMany(artist => artist.Albums).WithMany(album => album.Artists);
        builder.HasMany(artist => artist.Tracks).WithMany(track => track.Artists);
    }
}