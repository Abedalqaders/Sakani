using Application.Common.Interfaces.Expense;
using Application.Dto.Expense;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseAppService _expenseAppService;

        public ExpensesController(IExpenseAppService expenseAppService)
        {
            _expenseAppService = expenseAppService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDto dto, CancellationToken ct)
        {
            var id = await _expenseAppService.CreateExpenseAsync(dto, ct);
            return CreatedAtAction(nameof(GetExpenseById), new { id }, new { Id = id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken ct)
        {
            dto.ExpenseId = id;
            var result = await _expenseAppService.UpdateExpenseAsync(dto, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetExpenseById(Guid id, CancellationToken ct)
        {
            var result = await _expenseAppService.GetExpenseByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExpenses(CancellationToken ct)
        {
            var result = await _expenseAppService.GetAllExpensesAsync(ct);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken ct)
        {
            await _expenseAppService.DeleteExpenseAsync(id, ct);
            return NoContent();
        }

        [HttpGet("property/{propertyId:guid}")]
        public async Task<IActionResult> GetExpensesByProperty(Guid propertyId, CancellationToken ct)
        {
            var result = await _expenseAppService.GetExpensesByPropertyAsync(propertyId, ct);
            return Ok(result);
        }

        [HttpGet("unit/{unitId:guid}")]
        public async Task<IActionResult> GetExpensesByUnit(Guid unitId, CancellationToken ct)
        {
            var result = await _expenseAppService.GetExpensesByUnitAsync(unitId, ct);
            return Ok(result);
        }
    }
}