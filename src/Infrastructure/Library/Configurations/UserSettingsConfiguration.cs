using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(us => us.Id);

        builder.Property(us => us.Id).ValueGeneratedOnAdd();
        builder.Property(us => us.UserId).IsRequired();
        builder.Property(us => us.ProfileImageAttachmentId);
        builder.Property(us => us.ThemePrimaryColor).HasMaxLength(50);
        builder.Property(us => us.ThemeSecondaryColor).HasMaxLength(50);
        builder.Property(us => us.ThemeAccentColor).HasMaxLength(50);
        builder.Property(us => us.ThemeMode).HasMaxLength(50);

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.ProfileImageAttachment)
            .WithMany()
            .HasForeignKey(us => us.ProfileImageAttachmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable("UserSettings", "dbo");
    }
}

