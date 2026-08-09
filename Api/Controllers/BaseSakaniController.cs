using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseSakaniController : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
        }
    }
    protected Guid CurrentTenantId
    {
        get
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value;
            return Guid.TryParse(tenantIdClaim, out var id) ? id : Guid.Empty;
        }
    }
    protected bool IsSuperAdmin => User.IsInRole("SuperAdmin");

    protected bool HasActiveTenant => CurrentTenantId != Guid.Empty;
}