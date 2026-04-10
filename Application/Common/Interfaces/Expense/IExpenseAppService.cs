using Application.Dto.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Expense
{
    public interface IExpenseAppService
    {
        public Task<Guid> CreateExpenseAsync(CreateExpenseDto request,CancellationToken ct);
        public Task<ExpenseResponseDto> UpdateExpenseAsync(UpdateExpenseDto dto,CancellationToken cancellationToken);
        public Task<ExpenseResponseDto> GetExpenseByIdAsync(Guid id,CancellationToken ct);
        public Task<IReadOnlyList<ExpenseResponseDto>> GetAllExpensesAsync(CancellationToken ct);
        public Task DeleteExpenseAsync(Guid id,CancellationToken ct);
        public Task<IReadOnlyList<ExpenseResponseDto>> GetExpensesByPropertyAsync(Guid propertyId, CancellationToken ct);
        public Task<IReadOnlyList<ExpenseResponseDto>> GetExpensesByUnitAsync(Guid propertyId, CancellationToken ct);






    }
}
