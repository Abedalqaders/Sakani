using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Payment
{
    public class DashboardStatsDto
    {
       public decimal TotalExpectedMonth { set; get; }
       public decimal TotalCollectedMonth { set; get; }
       public decimal OccupancyRate { set; get; }
       public decimal ExpensesMonth { set; get; }

        public decimal NetIncomeMonth => TotalCollectedMonth - ExpensesMonth;
    }
}
