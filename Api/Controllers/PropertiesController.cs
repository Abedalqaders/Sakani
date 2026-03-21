using Application.Common.Interfaces;
using Application.Dto.Property;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyAppService _propertyService;

        public PropertiesController(IPropertyAppService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PropertyResponseDto>>> GetAllProperties(CancellationToken cancellationToken)
        {
            var properties = await _propertyService.GetAllPropertiesAsync(cancellationToken);
            if (properties.Count == 0)
            {
                return NotFound("No Property Found!");
            }
            return Ok(properties);
        }

        [HttpGet("{id:guid}", Name = "GetPropertyById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropertyResponseDto>> GetPropertyById(Guid id, CancellationToken cancellationToken)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id, cancellationToken);

            if (property is null)
                return NotFound($"Property with ID {id} was not found.");

            return Ok(property);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateProperty([FromBody] CreatePropertyDto dto, CancellationToken cancellationToken)
        {
            var propertyId = await _propertyService.CreatePropertyAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPropertyById), new { id = propertyId }, propertyId);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("The ID in the URL does not match the ID in the request body.");

            
                await _propertyService.UpdatePropertyAsync(dto, cancellationToken);
                return NoContent();
            
           
        }


        [HttpDelete("{id:guid}", Name = "DeleteProperty")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProperty(Guid id, CancellationToken cancellationToken)
        {
           
                await _propertyService.DeletePropertyAsync(id, cancellationToken);
                return NoContent();
            
            
        }
    }
}
