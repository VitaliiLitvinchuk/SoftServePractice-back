using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Actors;

namespace Infrastructure.Persistence.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ActorId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Surname).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Middlename).HasColumnType("varchar(255)");
        builder.Property(x => x.ImageUrl).IsRequired().HasColumnType("varchar(510)");

        builder.HasMany(x => x.Movies)
            .WithOne(x => x.Actor)
            .HasForeignKey(x => x.ActorId)
            .HasConstraintName("FK_Actor_MovieActor_ActorId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
