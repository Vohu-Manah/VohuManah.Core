using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VohuManah.Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<VohuManah.Domain.Library.User>
{
    public void Configure(EntityTypeBuilder<VohuManah.Domain.Library.User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(30);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(25);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);

        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

        builder.ToTable("LibraryUsers", "dbo");
    }
}


