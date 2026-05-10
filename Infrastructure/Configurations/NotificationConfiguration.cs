using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // اسم الجدول
            builder.ToTable("Notifications");

            // المفتاح الأساسي (موروث من BaseEntity)
            builder.HasKey(n => n.Id);

            // إعدادات الحقول
            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            // تخزين نوع الإشعار كرقم في قاعدة البيانات لتحسين الأداء
            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<int>();

            // العلاقات (Relationships)
            // 1. المستلم
            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); // يمنع حذف المستخدم إذا كان لديه إشعارات لتجنب فقدان السجلات

            // 2. المرسل
            builder.HasOne(n => n.Sender)
                .WithMany()
                .HasForeignKey(n => n.SenderId)
                .IsRequired(false) // لأن المرسل قد يكون النظام (null)
                .OnDelete(DeleteBehavior.SetNull); // إذا تم حذف المرسل، يصبح الحقل null ويبقى الإشعار

            // الفهارس (Indexes) لتحسين سرعة الاستعلامات بشكل ملحوظ
            // فهرس لجلب إشعارات مستخدم داخل Tenant مرتبة حسب الوقت
            builder.HasIndex(n => new { n.TenantId, n.UserId, n.CreatedAt });

            // فهرس للبحث السريع عن الإشعارات غير المقروءة
            builder.HasIndex(n => new { n.UserId, n.IsRead });

   
        }
    }
}