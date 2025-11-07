using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class PublicationConfiguration : IEntityTypeConfiguration<Publication>
{
    public void Configure(EntityTypeBuilder<Publication> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Concessionaire).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Editor).IsRequired().HasMaxLength(100);
        builder.Property(p => p.JournalNo).IsRequired().HasMaxLength(20);
        builder.Property(p => p.LanguageId).IsRequired();
        builder.Property(p => p.No).IsRequired().HasMaxLength(20);
        builder.Property(p => p.PublishDate).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PublishPlaceId).IsRequired();
        builder.Property(p => p.ResponsibleDirector).IsRequired().HasMaxLength(100);
        builder.Property(p => p.SubjectId).IsRequired();
        builder.Property(p => p.TypeId).IsRequired();
        builder.Property(p => p.Year).IsRequired().HasMaxLength(30);
        builder.Property(p => p.Period).IsRequired().HasMaxLength(20);

        builder.ToTable("Publication", "dbo");

        builder.HasOne(p => p.Language)
            .WithMany(l => l.Publications)
            .HasForeignKey(p => p.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Subject)
            .WithMany(s => s.Publications)
            .HasForeignKey(p => p.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.City)
            .WithMany(c => c.Publications)
            .HasForeignKey(p => p.PublishPlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PublicationType)
            .WithMany(pt => pt.Publications)
            .HasForeignKey(p => p.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


