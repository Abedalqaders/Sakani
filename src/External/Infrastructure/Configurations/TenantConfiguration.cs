using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal class TenantConfiguration: IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            // 1. الفلاتر العامة (Global Query Filters)
            builder.HasQueryFilter(t => !t.IsDeleted);

            // 2. تحويل الـ Enums
            builder.Property(t => t.Status).HasConversion<byte>();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.AddressCity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.AddressStreet)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.AddressRegion)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(256);
            builder.HasIndex(u => u.Email)
                .IsUnique();
            builder.Property(t => t.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
            // 3. العلاقات (Relationships)
            // الشركات والمستخدمين (Tenant -> Users)
         
        }
    }
}
