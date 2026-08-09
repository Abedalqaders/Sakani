using Application.Common.Interfaces; // تأكد من مسار الـ Interface عندك
using Application.Common.Interfaces.Accounting;
using Application.Common.Interfaces.Payment;
using Application.Dto.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentAppService _paymentService;
        private readonly IAccountingService _accountingRepo;
        public PaymentsController(IPaymentAppService paymentService, IAccountingService accountingService)
        {
            _paymentService = paymentService;
            _accountingRepo = accountingService;
        }
       
        [Authorize(Roles = "Renter")]
        [HttpPost("simulate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SimulatePayment([FromBody] PaymentSimulationDto dto, CancellationToken cancellationToken)
        {
            var transactionId = await _paymentService.PayWithCreditCardSimulatedAsync(dto, cancellationToken);

            return Ok(new
            {
                Message = "Payment completed successfully.",
                TransactionId = transactionId
            });
        }
       
        [Authorize(Roles = "Renter")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDetailsDto>> GetPaymentDetails(Guid id, CancellationToken ct)
        {
            var details = await _accountingRepo.GetPaymentDetailsAsync(id, ct);
            return Ok(details);
        }
    }
}