using VohuManah.Domain.PublicationTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VohuManah.Infrastructure.Database.Configurations;

internal sealed class PublicationTypeConfiguration : IEntityTypeConfiguration<PublicationType>
{
    public void Configure(EntityTypeBuilder<PublicationType> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Id).ValueGeneratedOnAdd();
        builder.Property(pt => pt.Title).IsRequired().HasMaxLength(50);

        builder.ToTable("PublicationType", "dbo");
    }
}


