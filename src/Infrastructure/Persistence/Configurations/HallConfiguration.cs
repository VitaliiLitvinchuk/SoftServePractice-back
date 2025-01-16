using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Halls;

namespace Infrastructure.Persistence.Configurations;

public class HallConfiguration : IEntityTypeConfiguration<Hall>
{
    public void Configure(EntityTypeBuilder<Hall> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new HallId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Capacity)
            .IsRequired()
            .HasColumnType("smallint");

        builder.HasMany(x => x.Seats)
            .WithOne(x => x.Hall)
            .HasForeignKey(x => x.HallId)
            .HasConstraintName("FK_Hall_Seat_HallId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sessions)
            .WithOne(x => x.Hall)
            .HasForeignKey(x => x.HallId)
            .HasConstraintName("FK_Hall_Session_HallId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
