using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Users;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.PasswordHash).IsRequired().HasColumnType("text");

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("FK_User_Role_RoleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.MovieRatings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_MovieRating_User_UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PurchaseHistories)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_PurchaseHistory_User_UserId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
