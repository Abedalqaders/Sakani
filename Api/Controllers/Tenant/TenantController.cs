using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;

[ApiController]
[Route("api/[controller]")]
[Authorize] // حماية عامة لجميع الـ Endpoints في الكنترولر
public class TenantsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TenantsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "TenantAccess")] 
    public async Task<ActionResult<TenantResponseDto>> GetById(Guid id)
    {
        // استخدام FirstOrDefaultAsync لضمان مرور الطلب عبر الـ Global Query Filter
        var tenant = await _context.Tenants
            .Where(t => t.Id == id)
            .Select(t => new TenantResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                PhoneNumber = t.PhoneNumber,
                AddressCity = t.AddressCity,
                Status = t.Status.ToString()
            })
            .FirstOrDefaultAsync();

        if (tenant == null)
        {
            return NotFound(new { Message = "Tenant not found or you don't have access." });
        }

        return Ok(tenant);
    }
}