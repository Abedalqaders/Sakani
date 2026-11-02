using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // 1. قيود البيانات (Data Constraints)

            // Amount: الدقة المالية ضرورية جداً هنا
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // التواريخ
            builder.Property(p => p.DueDate)
                .IsRequired();

            builder.Property(p => p.PaymentDate)
                .IsRequired(false); // يسمح بـ Null حتى يتم الدفع فعلياً

            builder.Property(p => p.ActualPaymentDate)
                .IsRequired(false); // تاريخ الدفع الفعلي اختياري

            // رقم العملية من بوابة الدفع (اختياري)
            builder.Property(p => p.TransactionId)
                .HasMaxLength(100)
                .IsRequired(false);

            // 2. تحويل الـ Enums لقيم عددية (Byte)
            builder.Property(p => p.PaymentStatus)
                .HasConversion<byte>();

            // 3. العلاقات (Relationships)

            // الدفعات والعقود (Many Payments -> One Contract)
            // نعرّف العلاقة هنا لأن الـ Foreign Key (ContractId) موجود في هذا الجدول
            builder.HasOne(p => p.Contract)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. الفلتر الخاص بـ TenantId يبقى في ApplicationDbContext
        }
    }
}