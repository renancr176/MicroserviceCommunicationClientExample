using Identity.IdentityDbContext.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Identity.IdentityDbContext.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.RememberPhrase)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.EmailConfirmationToken)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(e => e.ResetPasswordToken)
            .IsRequired(false)
            .HasMaxLength(255);

        #region Relationships

        builder.HasMany(e => e.RefreshTokens)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }
}