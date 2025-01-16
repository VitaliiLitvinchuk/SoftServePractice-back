using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.MoviesTags;
using Domain.Movies;
using Domain.Tags;

namespace Infrastructure.Persistence.Configurations;

public class MovieTagConfiguration : IEntityTypeConfiguration<MovieTag>
{
    public void Configure(EntityTypeBuilder<MovieTag> builder)
    {
        builder.HasKey(x => new { x.MovieId, x.TagId });
        builder.Property(x => x.MovieId).HasConversion(x => x.Value, x => new MovieId(x));
        builder.Property(x => x.TagId).HasConversion(x => x.Value, x => new TagId(x));

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.Tags)
            .HasConstraintName("FK_MovieTag_Movie_MovieId")
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.Movies)
            .HasConstraintName("FK_MovieTag_Tag_TagId")
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
