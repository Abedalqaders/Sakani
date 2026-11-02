using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            // 1. قيود البيانات (Data Constraints)
            builder.Property(c => c.RentAmount)
                .IsRequired()
                .HasColumnType("numeric(18,2)"); 

            builder.Property(c => c.StartDate).IsRequired();
            builder.Property(c => c.EndDate).IsRequired();
            builder.Property(c => c.PaymentFreq).IsRequired();

            // تحويل الـ Enums
            builder.Property(c => c.ContractStatus)
                .HasConversion<byte>();
            builder.Property(c => c.PaymentFreq)
                .HasConversion<byte>(); 

            
            builder.HasCheckConstraint("CK_Contracts_ValidDates", "start_date < end_date");

            builder.HasCheckConstraint("CK_Contracts_PositiveRent", "rent_amount > 0");

         
            builder.HasIndex(c => new { c.TenantId, c.UnitId });
            builder.HasIndex(c => new { c.TenantId, c.RenterId });

            builder.HasOne(c => c.Unit)
                .WithMany(u => u.Contracts)
                .HasForeignKey(c => c.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Renter)
                .WithMany(r => r.Contracts)
                .HasForeignKey(c => c.RenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}