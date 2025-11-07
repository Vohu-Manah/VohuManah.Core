using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ManagerName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PlaceId).IsRequired();
        builder.Property(p => p.Telephone).IsRequired().HasMaxLength(15);
        builder.Property(p => p.Website).IsRequired().HasMaxLength(100);

        builder.ToTable("Publisher", "dbo");
    }
}


