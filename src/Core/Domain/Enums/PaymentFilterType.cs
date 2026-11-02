using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PaymentFilterType : byte
    {
        All = 0,      // كل الدفعات
        Overdue = 1,  // المتأخرات فقط (Status = Overdue)
        Upcoming = 2  // القادمة (Status = Pending && Date > Now)
    }
}
