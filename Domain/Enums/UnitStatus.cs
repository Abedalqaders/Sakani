using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum UnitStatus:byte
    {
        Available = 1,
        Rented = 2,
        UnderMaintenance = 3,
        Reserved = 4
    }
}
