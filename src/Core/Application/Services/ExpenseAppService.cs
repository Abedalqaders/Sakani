using Application.Common.Interfaces.Expense;
using Application.Common.Interfaces.General;
using Application.Dto.Expense;
using Domain.Entities;

namespace Application.Services
{
    public class ExpenseAppService : IExpenseAppService
    {
        private readonly IExpenseRepository _expenseRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Property> _propertyRepo;
        private readonly IGenericRepository<Unit> _unitRepo;
        public ExpenseAppService(IExpenseRepository expenseRepo, IUnitOfWork unitOfWork, IGenericRepository<Property> propertyRepo, IGenericRepository<Unit> unitRepo)
        {
            _expenseRepo = expenseRepo;
            _unitOfWork = unitOfWork;
            _propertyRepo = propertyRepo;
            _unitRepo = unitRepo;
        }
        public async Task<Guid> CreateExpenseAsync(CreateExpenseDto dto, CancellationToken ct)
        {
            var property = await _propertyRepo.GetByIdAsync(dto.PropertyId, ct);
            if (property == null)
            {
                throw new KeyNotFoundException("Property not found.");
            }
            if (dto.UnitId.HasValue)
            {
                var unit = await _unitRepo.GetByIdAsync(dto.UnitId.Value, ct);
                if (unit == null || unit.PropertyId != dto.PropertyId)
                {
                    throw new InvalidOperationException("Unit not found or does not belong to the specified property.");
                }
            }
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Description = dto.Description,
                Amount = dto.Amount,
                ExpenseDate = DateTime.UtcNow,
                PropertyId = dto.PropertyId,
                UnitId = dto.UnitId,
                ExpenseType= dto.expenseType
            };
            _expenseRepo.Add(expense);
            await _unitOfWork.SaveChangesAsync(ct);
            return expense.Id;
        }
        public async Task<ExpenseResponseDto> UpdateExpenseAsync(UpdateExpenseDto dto, CancellationToken ct)
        {
            var expense = await _expenseRepo.GetByIdAsync(dto.ExpenseId, ct);
            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }
            if (dto.Amount.HasValue)
            {
                expense.Amount = dto.Amount.Value;
            }
            if (!string.IsNullOrEmpty(dto.Description))
            {
                expense.Description = dto.Description;
            }
            if (dto.ExpenseType.HasValue)
            {
                expense.ExpenseType = dto.ExpenseType.Value;
            }
            _expenseRepo.Update(expense);
            await _unitOfWork.SaveChangesAsync(ct);
            return new ExpenseResponseDto
            {
                ExpenseID = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                ExpenseDate = expense.ExpenseDate,
                ExpenseType = expense.ExpenseType,
                PropertyId = expense.PropertyId,
                UnitId = expense.UnitId
            };
        }
        public async Task<ExpenseResponseDto> GetExpenseByIdAsync(Guid id, CancellationToken ct)
        {
            var expense = await _expenseRepo.GetByIdAsync(id, ct);
            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }
            return new ExpenseResponseDto
            {
                ExpenseID = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                ExpenseDate = expense.ExpenseDate,
                ExpenseType = expense.ExpenseType,
                PropertyId = expense.PropertyId,
                UnitId = expense.UnitId
            };
        }
        public async Task<IReadOnlyList<ExpenseResponseDto>> GetAllExpensesAsync(CancellationToken ct)
        {
            var expenses = await _expenseRepo.GetAllAsync(ct);
            return expenses.Select(e => new ExpenseResponseDto
            {
                ExpenseID = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                ExpenseDate = e.ExpenseDate,
                ExpenseType = e.ExpenseType,
                PropertyId = e.PropertyId,
                UnitId = e.UnitId
            }).ToList();
        }
        public async Task DeleteExpenseAsync(Guid id,CancellationToken ct)
        {
            var expense = await _expenseRepo.GetByIdAsync(id,ct);
            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }
            _expenseRepo.Delete(expense);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<ExpenseResponseDto>> GetExpensesByPropertyAsync(Guid propertyId, CancellationToken ct)
        {
            var expenses = await _expenseRepo.GetExpensesByPropertyAsync(propertyId, ct);
            if (expenses == null || !expenses.Any())
            {
                throw new KeyNotFoundException("Expense not found.");
            }
            return expenses.Select(e => new ExpenseResponseDto
            {
                ExpenseID = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                ExpenseDate = e.ExpenseDate,
                ExpenseType = e.ExpenseType,
                PropertyId = e.PropertyId,
                UnitId = e.UnitId
            }).ToList();
        }
        public async Task<IReadOnlyList<ExpenseResponseDto>> GetExpensesByUnitAsync(Guid unitId, CancellationToken ct)
        {
            var expenses = await _expenseRepo.GetExpensesByUnitAsync(unitId, ct);
            if (expenses == null || !expenses.Any())
            {
                throw new KeyNotFoundException("Expense not found.");
            }
            return expenses.Select(e => new ExpenseResponseDto
            {
                ExpenseID = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                ExpenseDate = e.ExpenseDate,
                ExpenseType = e.ExpenseType,
                PropertyId = e.PropertyId,
                UnitId = e.UnitId
            }).ToList();
        }
    }
}
