using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(album => album.Title);
        builder.HasMany(album => album.Tracks).WithMany(track => track.Albums);
        builder.HasMany(album => album.Artists).WithMany(artist => artist.Albums);
    }
}