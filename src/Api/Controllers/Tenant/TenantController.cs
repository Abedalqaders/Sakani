using Api.Controllers;
using Application.Common.Interfaces.Tenant;
using Application.Dto.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sakani.API.Controllers;

[ApiController]
[Route("api/tenants")]
[Authorize]
public class TenantsController : BaseSakaniController
{
    private readonly ITenantAppService _tenantService;

    public TenantsController(ITenantAppService tenantService)
    {
        _tenantService = tenantService;
    }

   
    [HttpGet]
    [Authorize (Roles ="SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TenantResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tenants = await _tenantService.GetAllTenantsAsync(cancellationToken);
        return Ok(tenants); 
    }



    [HttpGet("{id:guid}", Name = "GetTenantById")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantResponseDto>> GetTenantById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(id, cancellationToken);

        if (tenant is null)
            return NotFound($"Tenant with ID {id} was not found."); 

        return Ok(tenant);
    }

    [HttpPost(Name = "CreateTenant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<Guid>> CreateTenant([FromBody] CreateTenantDto dto, CancellationToken cancellationToken)
    {
       
        var tenantId = await _tenantService.CreateTenantAsync(dto, cancellationToken);

        
        return CreatedAtAction(nameof(GetTenantById), new { id = tenantId }, tenantId);
    }

    // PUT: api/tenants/{id}
    [HttpPut("{id:guid}",Name = "Tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest("The ID in the URL does not match the ID in the request body.");
        await _tenantService.UpdateTenantAsync(dto, cancellationToken);
        return NoContent(); 
    }

  
    [HttpDelete("{id:guid}",Name = "DeleteTenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
    {
            await _tenantService.DeleteTenantAsync(id, cancellationToken);
            return NoContent();   
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Tenant")]
    public async Task<ActionResult<TenantResponseDto>> GetMyTenantInfo(CancellationToken cancellationToken)
    {

        if (CurrentTenantId == Guid.Empty)
            return BadRequest("You are not associated with any tenant.");

        var tenant = await _tenantService.GetTenantByIdAsync(CurrentTenantId, cancellationToken);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID {CurrentTenantId} was not found.");

        }
        else
        {
            return Ok(tenant);

        }
    }
}