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

            builder.Property(u => u.PasswordHashed)
                .IsRequired()
                .HasMaxLength(500);

            
            builder.HasIndex(u => new { u.TenantId, u.Email })
                .IsUnique()
                .HasFilter("is_deleted = false AND tenant_id IS NOT NULL");

      
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("is_deleted = false AND tenant_id IS NULL");

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}