using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Expense: BaseEntity
    {
        public Guid PropertyId { get; set; }
        public decimal Amount { get; set; }
        public ExpenseType  ExpenseType { get; set; }
        public DateTime ExpenseDate { get; set; }

        // Navigation Properties
        public Property Property { get; set; }
    }
}
