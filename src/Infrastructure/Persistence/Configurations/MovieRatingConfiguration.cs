using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.MoviesRatings;
using Domain.Movies;
using Domain.Users;
using Infrastructure.Constraints;

namespace Infrastructure.Persistence.Configurations;

public class MovieRatingConfiguration : IEntityTypeConfiguration<MovieRating>
{
    public void Configure(EntityTypeBuilder<MovieRating> builder)
    {
        builder.HasKey(x => new { x.MovieId, x.UserId });
        builder.Property(x => x.MovieId).HasConversion(x => x.Value, x => new MovieId(x));
        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.Rate)
            .IsRequired()
            .HasColumnType("smallint");

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.Ratings)
            .HasConstraintName("FK_MovieRating_Movie_MovieId")
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.MovieRatings)
            .HasConstraintName("FK_MovieRating_User_UserId")
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
