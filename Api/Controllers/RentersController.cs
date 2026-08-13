using Application.Common.Interfaces.Renter;
using Application.Dto.Renter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize (Roles ="Tenant")] 
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
           var renterId = await _renterService.CreateAsync(dto, ct);
           
           return CreatedAtAction(nameof(GetById), new { id = renterId }, renterId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<RenterResponseDto>>> GetAll(CancellationToken ct)
        {
            var renters = await _renterService.GetAllAsync(ct);
          
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