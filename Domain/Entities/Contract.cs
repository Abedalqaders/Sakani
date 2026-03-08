using Domain.Common;
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
        public int PaymentFreq { get; set; }
        public decimal RentAmount { get; set; }
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }
        public ContractStatus ContractStatus { get; set; }

        // Navigation Properties
        public Unit Unit { get; set; }
        public Renter Renter { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
