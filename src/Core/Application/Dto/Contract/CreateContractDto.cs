using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Contract
{
    public class CreateContractDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public decimal RentAmount { get; set; }
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public PaymentFrequency PaymentFreq { get; set; }

    }
}
