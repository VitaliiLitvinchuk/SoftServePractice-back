using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Seats;
using Domain.Halls;

namespace Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SeatId(x));

        builder.Property(x => x.Row)
            .IsRequired()
            .HasColumnType("smallint");

        builder.Property(x => x.Number)
            .IsRequired()
            .HasColumnType("smallint");

        builder.Property(x => x.HallId).HasConversion(x => x.Value, x => new HallId(x));

        builder.HasOne(x => x.Hall)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.HallId)
            .HasConstraintName("FK_Seat_Hall_HallId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tickets)
            .WithOne(x => x.Seat)
            .HasForeignKey(x => x.SeatId)
            .HasConstraintName("FK_Ticket_Seat_SeatId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
