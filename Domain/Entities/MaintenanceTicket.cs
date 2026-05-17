using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class TicketImage : TenantEntity
    {
        public string ImagePath { get; set; } = string.Empty;
        public Guid TicketId { get; set; }

        // Navigation Properties
        public MaintenanceTicket Ticket { get; set; } = null!;
    }

    public class MaintenanceTicket : TenantEntity
    {
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Open;
        public bool IsEscalationNotified { get; set; } = false;

        // Navigation Properties
        public Unit Unit { get; set; } = null!;
        public Renter Renter { get; set; } = null!;
        public ICollection<TicketImage> Images { get; set; } = new List<TicketImage>();
    }
}
