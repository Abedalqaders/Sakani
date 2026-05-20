using Application.Common.Interfaces.MaintenaceTicket;
using Application.Dto.MaintenanceTicket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/maintenance-tickets")]
    public class MaintenanceTicketsController : ControllerBase
    {
        private readonly IMaintenanceTicketAppService _ticketService;

        public MaintenanceTicketsController(IMaintenanceTicketAppService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        [Authorize(Roles = "Renter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateTicketDto dto, CancellationToken ct)
       {
            var ticketId = await _ticketService.CreateTicketAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = ticketId }, ticketId);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Renter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<TicketResponseDto>>> GetMy(CancellationToken ct)
        {
            var tickets = await _ticketService.GetMyTicketsAsync(ct);

            if (tickets.Count == 0)
                return NotFound(new { message = "No maintenance tickets found for the current renter." });

            return Ok(tickets);
        }

        [HttpGet("{id:guid}")]
        [Authorize] // „”„ÊÕ ··‹ Renter Ê «·‹ Tenant
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TicketResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var ticket = await _ticketService.GetByIdAsync(id, ct);

            if (ticket == null)
                return NotFound(new { message = $"Ticket with ID {id} was not found or access is denied." });

            return Ok(ticket);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Renter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "The ID in the URL does not match the ID in the request body." });

            await _ticketService.UpdateAsync(dto, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "Renter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            await _ticketService.CancelTicketAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{ticketId:guid}/images")]
        [Authorize(Roles = "Renter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UploadImage(Guid ticketId, IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Image file is required." });

            // ›Õ’ «·ÕÃ„ („À·« «·Õœ «·√ﬁ’Ï 5 „ÌÃ«»«Ì )
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size cannot exceed 5MB." });

            // ›Õ’ «·«„ œ«œ
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Only JPG and PNG images are allowed." });

            await using var stream = file.OpenReadStream();
            var imageUrl = await _ticketService.UploadImageAsync(ticketId, stream, file.FileName, ct);

            return Ok(new { url = imageUrl });
        }

        //  „  ÕÊÌ·Â« ·‹ GET Ê«” Œœ«„ FromQuery ﬂ‹ Best Practice
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<TicketResponseDto>>> GetAll([FromQuery] TicketFilterDto filter, CancellationToken ct)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(filter, ct);
            return Ok(tickets);
        }

        [HttpPatch("status")]
        [Authorize(Roles = "Tenant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTicketStatusDto dto, CancellationToken ct)
        {
            await _ticketService.UpdateStatusAsync(dto, ct);
            return NoContent();
        }
    }
}