using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.Property(m => m.Label).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Path).HasMaxLength(500);
        builder.Property(m => m.Icon).HasMaxLength(100);
        builder.Property(m => m.DisplayOrder).IsRequired();
        builder.Property(m => m.IsActive).IsRequired();
        builder.Property(m => m.IsSystem).IsRequired();

        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.DisplayOrder);
        builder.HasIndex(m => new { m.ParentId, m.DisplayOrder });

        builder.ToTable("MenuItem", "dbo");
    }
}

