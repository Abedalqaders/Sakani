using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MaintenanceTicketConfiguration : IEntityTypeConfiguration<MaintenanceTicket>
{
    public void Configure(EntityTypeBuilder<MaintenanceTicket> builder)
    {
        builder.Property(t => t.Subject).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(1000);

        builder.Property(t => t.TicketStatus).HasConversion<byte>();

        
        builder.HasOne(t => t.Unit)
            .WithMany()
            .HasForeignKey(t => t.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Renter)
            .WithMany()
            .HasForeignKey(t => t.RenterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}