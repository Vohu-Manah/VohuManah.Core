using VohuManah.Domain.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VohuManah.Infrastructure.Database.Configurations;

internal sealed class VisitLogConfiguration : IEntityTypeConfiguration<VisitLog>
{
    public void Configure(EntityTypeBuilder<VisitLog> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        builder.Property(v => v.Path).IsRequired().HasMaxLength(300);
        builder.HasIndex(v => v.VisitedAt);
        builder.ToTable("VisitLog", "dbo");
    }
}