using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Expense
{
    public class ExpenseResponseDto
    {
     public  Guid ExpenseID { get; set; }
     public Guid PropertyId { get; set; }
     public Guid? UnitId { get; set; }
     public decimal Amount { get; set; }
     public string? Description { get; set; } = string.Empty;
     public ExpenseType ExpenseType { get; set; }
     public DateTime ExpenseDate { get; set; }

    }
}
