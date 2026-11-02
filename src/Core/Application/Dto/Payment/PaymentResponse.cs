using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Payment
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; } 

        public Guid ContractId { get; set; }

        public string RenterName { get; set; }
        public string RenterPhoneNumber { get; set; }

       
        public string PropertyName { get; set; }
        public string UnitNo { get; set; }

        public int DaysUntilDue => (DueDate.Date - DateTime.UtcNow.Date).Days;
    }
}
