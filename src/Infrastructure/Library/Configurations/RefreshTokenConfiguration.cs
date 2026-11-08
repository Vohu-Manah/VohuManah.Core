using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Library;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id).ValueGeneratedOnAdd();
        builder.Property(rt => rt.UserName).IsRequired();
        builder.Property(rt => rt.Token).IsRequired();
        builder.Property(rt => rt.ExpiresAtUtc).IsRequired();
        builder.Property(rt => rt.RevokedAtUtc);

        builder.ToTable("RefreshTokens", "dbo");
    }
}


