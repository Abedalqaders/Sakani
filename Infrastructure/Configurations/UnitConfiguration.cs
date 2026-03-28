using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            // 1. قيود البيانات (Data Constraints)
            builder.Property(u => u.UnitNo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(u => u.Floor)
                .IsRequired()
                .HasMaxLength(10); // حماية الداتابيز من حجز مساحة عشوائية للنصوص

            builder.Property(u => u.RentPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // تحديد الدقة العشرية للأسعار (أفضل ممارسة)

            // 2. تحويل الـ Enums
            builder.Property(u => u.UnitStatus)
                .HasConversion<byte>(); // لتحسين الأداء وتقليل مساحة التخزين

            // 3. العلاقات (Relationships)
            builder.HasOne(u => u.Property)       // الوحدة تتبع عقار واحد
    .WithMany(p => p.Units)           // العقار له وحدات كثيرة
    .HasForeignKey(u => u.PropertyId)
    .OnDelete(DeleteBehavior.Restrict);


            // ملاحظة: علاقة العقار بالوحدات (Property -> Units) تمت إضافتها مسبقاً في PropertyConfiguration
            // لا حاجة لتكرارها هنا.

            // الفلتر الخاص بـ TenantId يبقى في ApplicationDbContext
        }
    }
}