using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(k => k.User)
            .WithMany(u => u.ApiKeys)
            .HasForeignKey(k => k.UserId);
    }
}