using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Tickets;
using Domain.Seats;
using Domain.Sessions;

namespace Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(x => new { x.Id });
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new TicketId(x));

        builder.Property(x => x.SeatId).HasConversion(x => x.Value, x => new SeatId(x));
        builder.Property(x => x.SessionId).HasConversion(x => x.Value, x => new SessionId(x));

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.HasOne(x => x.Seat)
            .WithMany(x => x.Tickets)
            .HasConstraintName("FK_Ticket_Seat_SeatId")
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Session)
            .WithMany(x => x.Tickets)
            .HasConstraintName("FK_Ticket_Session_SessionId")
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
