using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Payment
{
    public class PendingPaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string UnitNo { get; set; } = string.Empty;

        // هل الفاتورة متأخرة؟ (منطق بسيط للـ Frontend)
        public bool IsOverdue => DueDate < DateTime.UtcNow;

        // كم يوم ضايل للدفع (أو كم يوم تأخير)
        public int DaysRemaining => (DueDate.Date - DateTime.UtcNow.Date).Days;
    }
}
