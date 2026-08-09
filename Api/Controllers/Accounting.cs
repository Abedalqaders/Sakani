using Application.Common.Interfaces.Accounting;
using Application.Dto.Payment;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AccountingController : ControllerBase
    {
        private readonly IAccountingService _accountingService;

        public AccountingController(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        [HttpGet("Expected")]
        [Authorize(Roles = "Tenant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> GetExpectedPayments(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            CancellationToken ct)
        {
            var result = await _accountingService.GetExpectedPaymentsAsync(startDate, endDate, ct);
            return Ok(result);
        }

        [HttpGet("Overdue")]
        [Authorize(Roles = "Tenant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> GetOverduePayments(CancellationToken ct)
        {
            var result = await _accountingService.GetOverduePaymentsAsync(ct);
            return Ok(result);
        }

        [HttpGet("history")]
        [Authorize(Roles = "Renter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<PaymentHistoryResponseDto>>> GetMyPaymentHistory([FromQuery] PaymentFilterType filter, CancellationToken ct)
        {
            var history = await _accountingService.GetMyPaymentHistoryAsync(filter,ct);
            return Ok(history);
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "Tenant")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats(
            [FromQuery] int month,
            [FromQuery] int year,
            CancellationToken ct)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest("Invalid month. Month must be between 1 and 12.");
            }

            if (year < 2000 || year > 2100)
            {
                return BadRequest("Invalid year.");
            }

            var result = await _accountingService.GetDashboardStatsAsync(month, year, ct);
            if(result==null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("Expenses")]
        [Authorize(Roles = "Tenant")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetExpensesForRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            CancellationToken ct)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            if (startDate > endDate)
            {
                return BadRequest("Start date must be earlier than end date.");
            }
            if (startDate.Year < 2000 || startDate.Year > 2100 || endDate.Year < 2000 || endDate.Year > 2100)
            {
                return BadRequest("Invalid year.");
            }
            var expenses = await _accountingService.GetExpenseAmountRange(startDate, endDate, ct);
            return Ok(expenses);
        }
    }
}