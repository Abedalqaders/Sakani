using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Contract
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }

    public class ContractResponseDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public Guid UnitId { get; set; }
        public Guid RenterId { get; set; }
        public List<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
    }
}
