using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.MaintenanceTicket
{
    public class UpdateTicketDto
    {
        public Guid Id { get; set; } // ضروري عشان نعرف أي تذكرة بنعدل
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
