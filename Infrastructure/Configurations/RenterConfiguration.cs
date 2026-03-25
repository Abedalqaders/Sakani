using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RenterConfiguration : IEntityTypeConfiguration<Renter>
    {
        public void Configure(EntityTypeBuilder<Renter> builder)
        {
            // 1. قيود البيانات (Data Constraints)
            // NationalId: رقم الهوية عادة يكون طوله ثابت ومطلوب
            builder.Property(r => r.NationalId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.Description)
                .HasMaxLength(500); // تحديد طول الوصف لمنع NVARCHAR(MAX)

            // 2. العلاقات (Relationships)

            // المستأجر والمستخدم (User -> Renter)
            // علاقة 1:N (One User can be linked to One Renter record) 
            // مع السماح بكونها Null (IsRequired false)
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // المستأجر والعقود (Renter -> Contracts)
      

            // 3. الفلاتر
            // الفلتر الخاص بـ TenantId يبقى في DbContext
        }
    }
}