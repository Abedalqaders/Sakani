using Application.Common.Interfaces.General;
using Application.Dto.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Expense
{
    public interface IExpenseRepository:IGenericRepository<Domain.Entities.Expense>
    {
        public Task<IReadOnlyList<Domain.Entities.Expense>> GetExpensesByUnitAsync(Guid unitId, CancellationToken ct);
        public Task<IReadOnlyList<Domain.Entities.Expense>> GetExpensesByPropertyAsync(Guid propertyId, CancellationToken ct);

    }
}
