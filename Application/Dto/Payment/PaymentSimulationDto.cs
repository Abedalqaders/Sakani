using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Payment
{
    public class PaymentSimulationDto
    {
        public Guid PaymentId { get; set; }


        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty; // مثال: 12/26
        public string CVV { get; set; }= string.Empty;
    }
}
