using Application.Common.Interfaces.Accounting;
using Application.Dto.Payment;
using Microsoft.AspNetCore.Mvc;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingController : ControllerBase
    {
        private readonly IAccountingService _accountingService;

        public AccountingController(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        [HttpGet("Expected")]
        public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> GetExpectedPayments(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            CancellationToken ct)
        {
            var result = await _accountingService.GetExpectedPaymentsAsync(startDate, endDate, ct);
            return Ok(result);
        }

        [HttpGet("Overdue")]
        public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> GetOverduePayments(CancellationToken ct)
        {
            var result = await _accountingService.GetOverduePaymentsAsync(ct);
            return Ok(result);
        }

        [HttpGet("Stats")]
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
            return Ok(result);
        }
    }
}