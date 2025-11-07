using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Editor).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Corrector).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Isbn).IsRequired().HasMaxLength(20);
        builder.Property(b => b.LanguageId).IsRequired();
        builder.Property(b => b.No).IsRequired().HasMaxLength(800);
        builder.Property(b => b.PublishOrder).IsRequired().HasMaxLength(30);
        builder.Property(b => b.PublishPlaceId).IsRequired();
        builder.Property(b => b.PublishYear).IsRequired().HasMaxLength(30);
        builder.Property(b => b.PublisherId).IsRequired();
        builder.Property(b => b.SubjectId).IsRequired();
        builder.Property(b => b.Translator).IsRequired().HasMaxLength(200);
        builder.Property(b => b.VolumeCount).IsRequired();
        builder.Property(b => b.BookShelfRow).IsRequired().HasMaxLength(30);

        builder.ToTable("Book", "dbo");

        builder.HasOne(b => b.Language)
            .WithMany(l => l.Books)
            .HasForeignKey(b => b.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Subject)
            .WithMany(s => s.Books)
            .HasForeignKey(b => b.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.City)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.PublishPlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


