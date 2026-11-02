using Application.Common.Interfaces.Expense;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Expense>> GetExpensesByPropertyAsync(Guid propertyId, CancellationToken ct)
        {
            return await _context.Expenses
                .Where(x => x.PropertyId == propertyId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Expense>> GetExpensesByUnitAsync(Guid unitId, CancellationToken ct)
        {
            return await _context.Expenses
                .Where(e => e.UnitId == unitId)
                .ToListAsync(ct);
        }
    }
}