using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class Contract: TenantEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       
        public decimal RentAmount { get; set; }
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public PaymentFrequency PaymentFreq { get; set; }
        public bool IsExpirationReminderSent { get; set; } = false;
        public bool IsOverstayNotificationSent { get; set; } = false;

        // Navigation Properties
        public Unit Unit { get; set; }
        public Renter Renter { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
