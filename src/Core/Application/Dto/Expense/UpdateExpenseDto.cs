using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Expense
{
    public class UpdateExpenseDto
    {
        public Guid ExpenseId { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; } = string.Empty;
        public ExpenseType? ExpenseType { get; set; }
    }
}
