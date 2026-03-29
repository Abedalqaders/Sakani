using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Contract
{
    public class ContractBasicResponseDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }

    }
}
