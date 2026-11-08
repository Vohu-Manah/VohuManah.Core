using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class GapConfiguration : IEntityTypeConfiguration<Gap>
{
    public void Configure(EntityTypeBuilder<Gap> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).ValueGeneratedOnAdd();
        builder.Property(g => g.Note).IsRequired().HasMaxLength(200);
        builder.Property(g => g.State).IsRequired();
        builder.Property(g => g.Title).IsRequired().HasMaxLength(100);

        builder.ToTable("Gap", "dbo");
    }
}


