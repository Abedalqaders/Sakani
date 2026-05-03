using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Expense: TenantEntity
    {
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }
        public decimal Amount { get; set; }
        public ExpenseType  ExpenseType { get; set; }
        public DateTime ExpenseDate { get; set; }   
        public string? Description { get; set; }= string.Empty;

        // Navigation Properties
        public  Unit Unit { get; set; }
        public Property Property { get; set; }
    }
}
