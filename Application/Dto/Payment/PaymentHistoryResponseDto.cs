using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Payment
{
    public class PaymentHistoryResponseDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; } // عن أي شهر هاي الدفعة؟
        public DateTime PaymentDate { get; set; } // متى دفعها فعلياً؟
        public string TransactionId { get; set; } = string.Empty; // رقم إثبات الدفع

        // تفاصيل العقار عشان لو المستأجر مأجر أكثر من شقة
        public string PropertyName { get; set; } = string.Empty;
        public string UnitNo { get; set; } = string.Empty;
    }
}
