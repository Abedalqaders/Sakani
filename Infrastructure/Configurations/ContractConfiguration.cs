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

            // RentAmount: تحديد الدقة العشرية للقيم المالية
            builder.Property(c => c.RentAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // التواريخ ووتيرة الدفع
            builder.Property(c => c.StartDate).IsRequired();
            builder.Property(c => c.EndDate).IsRequired();
            builder.Property(c => c.PaymentFreq).IsRequired();

            // 2. تحويل الـ Enums
            builder.Property(c => c.ContractStatus)
                .HasConversion<byte>();

            // 3. العلاقات (الـ Foreign Keys موجودة في هذا الجدول)

            // العقد والوحدة (Many Contracts -> One Unit)
            builder.HasOne(c => c.Unit)
                .WithMany(u => u.Contracts)
                .HasForeignKey(c => c.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // العقد والمستأجر (Many Contracts -> One Renter)
            builder.HasOne(c => c.Renter)
                .WithMany(r => r.Contracts)
                .HasForeignKey(c => c.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

          
          

            // الفلتر الخاص بالـ TenantId والـ Soft Delete يبقى في DbContext
        }
    }
}