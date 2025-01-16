using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Tags;

namespace Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new TagId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");

        builder.HasMany(x => x.Movies)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId)
            .HasConstraintName("FK_Tag_MovieTag_TagId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Genres)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId)
            .HasConstraintName("FK_Tag_MovieGenre_TagId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
