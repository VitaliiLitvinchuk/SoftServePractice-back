using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PurchaseHistories;
using Domain.Users;
using Infrastructure.Constraints;
using Domain.Tickets;

namespace Infrastructure.Persistence.Configurations;

public class PurchaseHistoryConfiguration : IEntityTypeConfiguration<PurchaseHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new PurchaseHistoryId(x));

        builder.Property(x => x.PurchasedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));
        builder.Property(x => x.TicketId).HasConversion(x => x.Value, x => new TicketId(x));

        builder.HasOne(x => x.User)
            .WithMany(x => x.PurchaseHistories)
            .HasConstraintName("FK_PurchaseHistory_User_UserId")
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.PurchaseHistories)
            .HasConstraintName("FK_PurchaseHistory_Ticket_TicketId")
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
