using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            // 1. تحديد الجدول (اختياري لكن يفضل للتنظيم)
            builder.ToTable("Expenses");

            // 2. قيود البيانات (Data Constraints)

            // Amount: الأسعار والمبالغ دائماً حدد لها الدقة لضمان صحة الحسابات
            builder.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // ExpenseDate: يفضل دائماً جعل التاريخ مطلوباً
            builder.Property(e => e.ExpenseDate)
                .IsRequired();

            // 3. تحويل الـ Enums لقيم عددية (SmallInt/Byte)
            builder.Property(e => e.ExpenseType)
                .HasConversion<byte>();

            // 4. العلاقات (Relationships)
            // علاقة (Property -> Expenses) تم تعريفها في PropertyConfiguration 
            // لكن يفضل كتابة الطرف الثاني هنا للتأكيد (أو الاعتماد على تعريف واحد فقط)
            builder.HasOne(e => e.Property)
                .WithMany(p => p.Expenses)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. الفلتر الخاص بالـ TenantId سيتم تعريفه في الـ DbContext كما اتفقنا سابقاً
        }
    }
}