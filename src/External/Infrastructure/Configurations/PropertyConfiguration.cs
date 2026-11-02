using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            // 1. قيود البيانات (Data Constraints)
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.City)
                .IsRequired()
                .HasMaxLength(100); // تمت إضافتها لحماية الداتابيز - عدل الفالديتور ليتطابق معها

            builder.Property(p => p.BuildingNo)
                .IsRequired()
                .HasMaxLength(20);

            // 2. تحويل الـ Enums
            builder.Property(p => p.PropertyType)
                .HasConversion<byte>();

            // 3. العلاقات (Relationships)
            // العقارات والوحدات (Property -> Units)
            builder.HasMany(p => p.Units)
                .WithOne(u => u.Property)
                .HasForeignKey(u => u.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

      
            // ملاحظة: لم نضف HasQueryFilter هنا لأنه يعتمد على _tenantId 
            // وسيبقى في ملف ApplicationDbContext كما اتفقنا.
        }
    }
}