using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.MoviesActors;
using Domain.Actors;
using Domain.Movies;

namespace Infrastructure.Persistence.Configurations;

public class MovieActorConfiguration : IEntityTypeConfiguration<MovieActor>
{
    public void Configure(EntityTypeBuilder<MovieActor> builder)
    {
        builder.HasKey(x => new { x.ActorId, x.MovieId });
        builder.Property(x => x.ActorId).HasConversion(x => x.Value, x => new ActorId(x));
        builder.Property(x => x.MovieId).HasConversion(x => x.Value, x => new MovieId(x));

        builder.HasOne(x => x.Actor)
            .WithMany(x => x.Movies)
            .HasConstraintName("FK_MovieActor_Actor_ActorId")
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.Actors)
            .HasConstraintName("FK_MovieActor_Movie_MovieId")
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
