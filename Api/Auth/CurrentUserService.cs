using Application.Common.Interfaces;

using System.Security.Claims;

namespace Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }
    public Guid? TenantId =>
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("tenantId")?.Value, out var id) ? id : null;
}