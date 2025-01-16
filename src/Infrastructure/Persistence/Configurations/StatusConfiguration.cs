using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Statuses;

namespace Infrastructure.Persistence.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new StatusId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");

        builder.HasMany(x => x.Sessions)
            .WithOne(x => x.Status)
            .HasForeignKey(x => x.StatusId)
            .HasConstraintName("FK_Session_Status_StatusId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
