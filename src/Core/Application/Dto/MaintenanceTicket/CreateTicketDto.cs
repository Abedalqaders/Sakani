using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.MaintenanceTicket
{
    public class CreateTicketDto
    {
        public Guid? UnitId { get; set; } 
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
