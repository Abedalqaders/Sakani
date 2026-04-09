using System.Security.Claims;
using Application.Common.Interfaces.Tenant;
using Microsoft.AspNetCore.Http;

namespace Sakani.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetTenantId()
    {
        
        var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenantId")?.Value;

        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            return tenantId;
        }

        return null; 
    }
}