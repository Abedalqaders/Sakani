using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ExpenseType:byte
    {
        Maintenance = 1, 
        Utility = 2,     
        Tax = 3,         
        Insurance = 4,  
        Other = 5           
    }
}
