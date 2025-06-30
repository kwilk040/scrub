using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class InviteCodeConfiguration : IEntityTypeConfiguration<InviteCode>
{
    public void Configure(EntityTypeBuilder<InviteCode> builder)
    {
        builder
            .HasOne(i => i.CreatedByUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.UsedByUser)
            .WithMany()
            .HasForeignKey(i => i.UsedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}