using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Sessions;
using Infrastructure.Constraints;
using Domain.Statuses;
using Domain.Movies;
using Domain.Halls;

namespace Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SessionId(x));

        builder.Property(x => x.StartAt)
            .IsRequired()
            .HasConversion(new DateTimeUtcConverter());

        builder.Property(x => x.EndAt)
            .IsRequired()
            .HasConversion(new DateTimeUtcConverter());

        builder.Property(x => x.StatusId).HasConversion(x => x.Value, x => new StatusId(x));
        builder.Property(x => x.MovieId).HasConversion(x => x.Value, x => new MovieId(x));
        builder.Property(x => x.HallId).HasConversion(x => x.Value, x => new HallId(x));

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Sessions)
            .HasConstraintName("FK_Session_Status_StatusId")
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.Sessions)
            .HasConstraintName("FK_Session_Movie_MovieId")
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Hall)
            .WithMany(x => x.Sessions)
            .HasConstraintName("FK_Session_Hall_HallId")
            .HasForeignKey(x => x.HallId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tickets)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .HasConstraintName("FK_Session_Ticket_SessionId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
