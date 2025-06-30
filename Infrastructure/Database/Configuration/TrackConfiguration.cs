using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(track => track.Title);
        builder.HasMany(track => track.Artists).WithMany(artist => artist.Tracks);
        builder.HasMany(track => track.Albums).WithMany(album => album.Tracks);
    }
}