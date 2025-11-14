using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).ValueGeneratedOnAdd();
        builder.Property(rp => rp.RoleId).IsRequired();
        builder.Property(rp => rp.EndpointName).IsRequired().HasMaxLength(200);
        
        // ایجاد index برای جستجوی سریع‌تر
        builder.HasIndex(rp => new { rp.RoleId, rp.EndpointName }).IsUnique();
        
        // Foreign key به Role
        builder.HasOne(rp => rp.Role)
            .WithMany()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ToTable("RolePermission", "dbo");
    }
}

