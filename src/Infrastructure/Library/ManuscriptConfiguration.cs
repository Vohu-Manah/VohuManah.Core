using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class ManuscriptConfiguration : IEntityTypeConfiguration<Manuscript>
{
    public void Configure(EntityTypeBuilder<Manuscript> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Author).IsRequired().HasMaxLength(200);
        builder.Property(m => m.GapId).IsRequired();
        builder.Property(m => m.LanguageId).IsRequired();
        builder.Property(m => m.PageCount).IsRequired();
        builder.Property(m => m.Size).IsRequired().HasMaxLength(100);
        builder.Property(m => m.SubjectId).IsRequired();
        builder.Property(m => m.VersionNo).IsRequired().HasMaxLength(20);
        builder.Property(m => m.WritingPlaceId).IsRequired();
        builder.Property(m => m.Year).IsRequired().HasMaxLength(30);

        builder.ToTable("Manuscript", "dbo");

        builder.HasOne(m => m.Language)
            .WithMany(l => l.Manuscripts)
            .HasForeignKey(m => m.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Subject)
            .WithMany(s => s.Manuscripts)
            .HasForeignKey(m => m.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.City)
            .WithMany(c => c.Manuscripts)
            .HasForeignKey(m => m.WritingPlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Gap)
            .WithMany(g => g.Manuscripts)
            .HasForeignKey(m => m.GapId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


