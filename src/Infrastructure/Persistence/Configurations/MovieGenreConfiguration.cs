using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.MoviesGenres;
using Domain.Genres;
using Domain.Movies;

namespace Infrastructure.Persistence.Configurations;

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> builder)
    {
        builder.HasKey(x => new { x.GenreId, x.MovieId });
        builder.Property(x => x.GenreId).HasConversion(x => x.Value, x => new GenreId(x));
        builder.Property(x => x.MovieId).HasConversion(x => x.Value, x => new MovieId(x));

        builder.HasOne(x => x.Genre)
            .WithMany(x => x.Movies)
            .HasConstraintName("FK_MovieGenre_Genre_GenreId")
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.Genres)
            .HasConstraintName("FK_MovieGenre_Movie_MovieId")
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
