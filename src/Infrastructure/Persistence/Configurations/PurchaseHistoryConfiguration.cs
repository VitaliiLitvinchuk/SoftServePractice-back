using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PurchaseHistories;
using Domain.Users;
using Domain.Movies;
using Infrastructure.Constraints;

namespace Infrastructure.Persistence.Configurations;

public class PurchaseHistoryConfiguration : IEntityTypeConfiguration<PurchaseHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new PurchaseHistoryId(x));

        builder.Property(x => x.PurchasedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));
        builder.Property(x => x.MovieId).HasConversion(x => x.Value, x => new MovieId(x));

        builder.HasOne(x => x.User)
            .WithMany(x => x.PurchaseHistories)
            .HasConstraintName("FK_PurchaseHistory_User_UserId")
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.PurchaseHistories)
            .HasConstraintName("FK_PurchaseHistory_Movie_MovieId")
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
