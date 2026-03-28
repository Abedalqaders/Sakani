using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PaymentFrequency : byte
    {
        Monthly = 1,
        Quarterly = 3, 
        SemiAnnually = 6, 
        Yearly = 12
    }
}
