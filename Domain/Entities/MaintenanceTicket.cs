using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Entities
{
    public class Image :TenantEntity
    {
        public string ImagePath { get; set; }
        public Guid TicketId { get; set; }

        // Navigation Properties
        public MaintenanceTicket Ticket { get; set; }
    }
    public class MaintenanceTicket:TenantEntity
    {
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public TicketStatus TicketStatus { get; set; }

        // Navigation Properties
        public Unit Unit { get; set; }
        public Renter Renter { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
