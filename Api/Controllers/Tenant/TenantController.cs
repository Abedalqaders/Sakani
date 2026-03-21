using Application.Common.Interfaces;
using Application.Dto.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakani.Application.Common.Interfaces;

namespace Sakani.API.Controllers;

[ApiController]
[Route("api/tenants")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantAppService _tenantService;

    public TenantsController(ITenantAppService tenantService)
    {
        _tenantService = tenantService;
    }

   
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TenantResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tenants = await _tenantService.GetAllTenantsAsync(cancellationToken);
        if (tenants.Count == 0)
        {
            return NotFound("No Tenant Found!");
        }
        return Ok(tenants); 
    }

    // GET: api/tenants/{id}
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}", Name = "GetTenantById")]
    public async Task<ActionResult<TenantResponseDto>> GetTenantById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(id, cancellationToken);

        if (tenant is null)
            return NotFound($"Tenant with ID {id} was not found."); 

        return Ok(tenant);
    }

    // POST: api/tenants
    [HttpPost(Name = "CreateTenant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
    {
    
            await _tenantService.DeleteTenantAsync(id, cancellationToken);
            return NoContent(); 
       
    }
}