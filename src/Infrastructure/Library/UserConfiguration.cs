using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class UserConfiguration : IEntityTypeConfiguration<Domain.Library.User>
{
    public void Configure(EntityTypeBuilder<Domain.Library.User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(30);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(200); // Increased for password hash
        builder.Property(u => u.Name).IsRequired().HasMaxLength(25);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        
        builder.HasIndex(u => u.UserName).IsUnique();
        
        builder.ToTable("LibraryUsers", "dbo");
    }
}


