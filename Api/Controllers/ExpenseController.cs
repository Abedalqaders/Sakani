using Application.Common.Interfaces.Expense;
using Application.Dto.Expense; // Assuming you have standard DTO names here
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Tenant")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseAppService _expenseAppService;

        public ExpensesController(IExpenseAppService expenseAppService)
        {
            _expenseAppService = expenseAppService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateExpense([FromBody] CreateExpenseDto dto, CancellationToken ct)
        {
            var id = await _expenseAppService.CreateExpenseAsync(dto, ct);
            return CreatedAtAction(nameof(GetExpenseById), new { id = id }, id);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken ct)
        {
            if (id != dto.ExpenseId)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            await _expenseAppService.UpdateExpenseAsync(dto, ct);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseResponseDto>> GetExpenseById(Guid id, CancellationToken ct)
        {
            var result = await _expenseAppService.GetExpenseByIdAsync(id, ct);
            if (result == null)
            {
                return NotFound(); // Global middleware handles problem details
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExpenseResponseDto>>> GetAllExpenses(CancellationToken ct)
        {
            var result = await _expenseAppService.GetAllExpensesAsync(ct);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteExpense(Guid id, CancellationToken ct)
        {
            await _expenseAppService.DeleteExpenseAsync(id, ct);
            return NoContent();
        }

        [HttpGet("property/{propertyId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExpenseResponseDto>>> GetExpensesByProperty(Guid propertyId, CancellationToken ct)
        {
            var result = await _expenseAppService.GetExpensesByPropertyAsync(propertyId, ct);
            return Ok(result);
        }

        [HttpGet("unit/{unitId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExpenseResponseDto>>> GetExpensesByUnit(Guid unitId, CancellationToken ct)
        {
            var result = await _expenseAppService.GetExpensesByUnitAsync(unitId, ct);
            return Ok(result);
        }
    }
}