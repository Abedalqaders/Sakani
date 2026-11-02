using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.MaintenanceTicket
{
    public class UpdateTicketStatusDto
    {
        public Guid TicketId { get; set; }

        // نستخدم الـ Enum الخاص بك هنا لتجنب إدخال نصوص خاطئة
        public TicketStatus NewStatus { get; set; }
    }
}
