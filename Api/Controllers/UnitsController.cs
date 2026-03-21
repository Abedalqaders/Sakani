using Application.Common.Interfaces;
using Application.Dto.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sakani.API.Controllers;

[ApiController]
[Route("api/units")]
[Authorize (Roles ="Tenant")]
public class UnitsController : ControllerBase
{
    private readonly IUnitAppService _unitService;

    public UnitsController(IUnitAppService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<UnitResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var units = await _unitService.GetAllAsync(cancellationToken);

        if (units.Count == 0)
        {
            return NotFound("No Units Found!");
        }

        return Ok(units);
    }

    [HttpGet("{id:guid}", Name = "GetUnitById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnitResponseDto>> GetUnitById(Guid id, CancellationToken cancellationToken)
    {
        var unit = await _unitService.GetByIdAsync(id, cancellationToken);

        if (unit is null)
            return NotFound($"Unit with ID {id} was not found.");

        return Ok(unit);
    }

    [HttpGet("property/{propertyId:guid}", Name = "GetUnitsByPropertyId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<UnitResponseDto>>> GetUnitsByPropertyId(Guid propertyId, CancellationToken cancellationToken)
    {
        var units = await _unitService.GetUnitsByPropertyIdAsync(propertyId, cancellationToken);

        if (units.Count == 0)
        {
            return NotFound($"No units found for Property ID {propertyId}.");
        }

        return Ok(units);
    }

    [HttpPost(Name = "CreateUnit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // ضروري لأن السيرفيس ترمي KeyNotFoundException
    [ProducesResponseType(StatusCodes.Status403Forbidden)] // في حال رميت UnauthorizedAccessException من السيرفيس
    public async Task<ActionResult<Guid>> CreateUnit([FromBody] CreateUnitDto dto, CancellationToken cancellationToken)
    {

        var unitId = await _unitService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetUnitById), new { id = unitId }, unitId);
    }

    [HttpPut("{id:guid}", Name = "UpdateUnit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UpdateUnitDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest("The ID in the URL does not match the ID in the request body.");

        try
        {
            await _unitService.UpdateAsync(dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception here in a real production scenario
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the unit.");
        }
    }

    [HttpDelete("{id:guid}", Name = "DeleteUnit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUnit(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _unitService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}