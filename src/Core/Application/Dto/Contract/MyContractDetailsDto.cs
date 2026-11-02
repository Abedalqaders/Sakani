using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Contract
{
    public class MyContractDetailsDto
    {
        public Guid ContractId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        public ContractStatus ContractStatus { get; set; }

        // معلومات تهم المستأجر
        public string UnitNo { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
    }
}
