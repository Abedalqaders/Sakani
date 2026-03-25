using Application.Common.Interfaces;
using Application.Dto.Contract;
using Application.Dto.Renter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize (Roles ="Tenant")] // Requires a valid JWT token
    public class RentersController : ControllerBase
    {
        private readonly IRenterAppService _renterService;

        public RentersController(IRenterAppService renterService)
        {
            _renterService = renterService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateRenterDto dto, CancellationToken ct)
        {
            
                // The service handles logic and returns the new Renter ID
                var renterId = await _renterService.CreateAsync(dto, ct);

                // Returns 201 Created with a link to the GetById endpoint
                return CreatedAtAction(nameof(GetById), new { id = renterId }, renterId);
            
           
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<RenterResponseDto>>> GetAll(CancellationToken ct)
        {
            var renters = await _renterService.GetAllAsync(ct);

            if (renters == null || !renters.Any())
            {
                return NotFound(new { message = "No renters found for this tenant." });
            }

            return Ok(renters);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RenterResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var renter = await _renterService.GetByIdAsync(id, ct);

            if (renter == null)
            {
                return NotFound(new { message = $"Renter with ID {id} not found." });
            }

            return Ok(renter);
        }
    }
}