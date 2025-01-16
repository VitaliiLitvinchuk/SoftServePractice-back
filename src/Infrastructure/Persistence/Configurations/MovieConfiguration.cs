using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Movies;

namespace Infrastructure.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new MovieId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Duration).IsRequired().HasColumnType("bigint");
        builder.Property(x => x.TrailerUrl).IsRequired().HasColumnType("varchar(510)");

        builder.HasMany(x => x.Genres)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .HasConstraintName("FK_Movie_MovieGenre_MovieId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Tags)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .HasConstraintName("FK_Movie_MovieTag_MovieId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Actors)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .HasConstraintName("FK_Movie_MovieActor_MovieId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sessions)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .HasConstraintName("FK_Movie_Session_MovieId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Ratings)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .HasConstraintName("FK_Movie_Rating_MovieId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PurchaseHistories)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .HasConstraintName("FK_Movie_PurchaseHistory_MovieId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
