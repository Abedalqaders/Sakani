using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.MaintenanceTicket
{
    public class TicketResponseDto
    {
        public Guid Id { get; set; }
        public Guid UnitId { get; set; }

        // من الجيد إرسال رقم الوحدة (الشقة) كاسم جاهز للواجهة بدل إجبار الـ Frontend على جلبه
        public string UnitNo { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // تحويل الـ Enum إلى نص (String) ليكون مفهوماً للـ Frontend (مثال: "InProgress")
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // قائمة الصور المرفقة بالتذكرة
        public List<TicketImageDto> Images { get; set; } = new();
    }
    public class TicketImageDto
    {
        public Guid Id { get; set; }

        // يفضل تسميته Url لأنه سيعرض في الواجهة (حتى لو كان مساراً محلياً)
        public string ImageUrl { get; set; } = string.Empty;
    }
}
