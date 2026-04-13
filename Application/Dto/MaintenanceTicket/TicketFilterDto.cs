using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.MaintenanceTicket
{
    public class TicketFilterDto
    {
        public TicketStatus? Status { get; set; }
        public Guid? UnitId { get; set; }
        public Guid? RenterId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
