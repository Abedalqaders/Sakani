using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
   public enum TenantStatus: byte
    {
        Active=1,
        Suspended=2,
        Inactive=3
    }
}
