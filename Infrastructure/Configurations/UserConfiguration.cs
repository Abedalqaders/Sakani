using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // 1. قيود البيانات (Data Constraints)
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHashed)
                .IsRequired()
                .HasMaxLength(500); // طول مناسب لتخزين الـ Hashes المعقدة (BCrypt/Argon2)

            // 2. العلاقات (Relationships)

            // المستخدم والدور (Many Users -> One Role)
            builder.HasOne(u => u.Role)
                .WithMany() // إذا لم يكن هناك ICollection<User> في كلاس Role
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // المستخدم والشركة (Many Users -> One Tenant)
            // نضعها هنا لأن الـ Foreign Key (TenantId) موجود في جدول الـ User
            builder.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .IsRequired(false) // يسمح بوجود Super Admin لا يتبع لـ Tenant معين
                .OnDelete(DeleteBehavior.Restrict);

            // 3. الفلاتر
            // فلتر !IsDeleted يبقى في الـ DbContext كما اتفقنا للـ Users
        }
    }
}