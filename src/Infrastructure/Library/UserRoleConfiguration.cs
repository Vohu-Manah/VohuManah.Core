using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).ValueGeneratedOnAdd();
        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.UserName).IsRequired().HasMaxLength(30);
        builder.Property(ur => ur.RoleId).IsRequired();
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        builder.ToTable("UserRole", "dbo");
    }
}


