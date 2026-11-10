using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class AppInfoConfiguration : IEntityTypeConfiguration<AppInfo>
{
    public void Configure(EntityTypeBuilder<AppInfo> builder)
    {
        builder.HasKey(ai => ai.Id);

        builder.Property(ai => ai.Id).ValueGeneratedOnAdd();
        builder.Property(ai => ai.AppName).IsRequired().HasMaxLength(100);
        builder.Property(ai => ai.Version).IsRequired().HasMaxLength(50);
        builder.Property(ai => ai.Author).IsRequired().HasMaxLength(100);
        builder.Property(ai => ai.Description).HasMaxLength(1000);
        builder.Property(ai => ai.Copyright).HasMaxLength(200);
        builder.Property(ai => ai.License).HasMaxLength(100);
        builder.Property(ai => ai.Website).HasMaxLength(200);
        builder.Property(ai => ai.SupportEmail).HasMaxLength(100);

        builder.ToTable("AppInfo", "dbo");
    }
}


