using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class RoleEndpointConfiguration : IEntityTypeConfiguration<RoleEndpoint>
{
    public void Configure(EntityTypeBuilder<RoleEndpoint> builder)
    {
        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).ValueGeneratedOnAdd();
        builder.Property(rp => rp.RoleId).IsRequired();
        builder.Property(rp => rp.EndpointName).IsRequired().HasMaxLength(200);

        builder.HasIndex(rp => new { rp.RoleId, rp.EndpointName }).IsUnique();

        builder.HasOne(rp => rp.Role)
            .WithMany()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("RolePermission", "dbo");
    }
}

