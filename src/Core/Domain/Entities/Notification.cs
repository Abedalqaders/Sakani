using Domain.Common;
using Domain.Enums;
using System;

namespace Domain.Entities
{
    public class Notification : TenantEntity
    {
        // المستلم (المستخدم الذي سيظهر له الإشعار)
        public Guid UserId { get; set; }

        // المرسل (يمكن أن يكون فارغا إذا كان الإشعار تلقائيا من النظام)
        public Guid? SenderId { get; set; }

        // المحتوى
        public string Title { get; set; }
        public string Message { get; set; }

        // التصنيف والربط
        public NotificationType Type { get; set; }
        public Guid? ReferenceId { get; set; }

        // حالة القراءة
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public User Sender { get; set; } // في حال احتجت لجلب بيانات المرسل مثل اسمه أو صورته
    }
}