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
                .IsRequired(false)
                .HasMaxLength(500);


            builder.HasIndex(r => new { r.TenantId, r.NationalId })
       .IsUnique();

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}