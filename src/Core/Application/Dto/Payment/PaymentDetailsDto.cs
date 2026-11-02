using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Payment
{
   public  class PaymentDetailsDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string UnitNo { get; set; } = string.Empty;
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
    }
}
