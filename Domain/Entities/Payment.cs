using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment : TenantEntity
    {
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ActualPaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public Guid ContractId { get; set; }
        public Contract Contract { get; set; }  
        public bool IsOverdueNotificationSent { get; set; } = false;
    }
}
