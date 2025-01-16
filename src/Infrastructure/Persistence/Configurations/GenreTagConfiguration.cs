using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.GenresTags;
using Domain.Genres;
using Domain.Tags;

namespace Infrastructure.Persistence.Configurations;

public class GenreTagConfiguration : IEntityTypeConfiguration<GenreTag>
{
    public void Configure(EntityTypeBuilder<GenreTag> builder)
    {
        builder.HasKey(x => new { x.GenreId, x.TagId });
        builder.Property(x => x.GenreId).HasConversion(x => x.Value, x => new GenreId(x));
        builder.Property(x => x.TagId).HasConversion(x => x.Value, x => new TagId(x));

        builder.HasOne(x => x.Genre)
            .WithMany(x => x.Tags)
            .HasConstraintName("FK_GenreTag_Genre_GenreId")
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.Genres)
            .HasConstraintName("FK_GenreTag_Tag_TagId")
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
