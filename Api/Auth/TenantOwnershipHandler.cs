using Microsoft.AspNetCore.Authorization;

public class TenantOwnershipHandler : AuthorizationHandler<TenantOwnershipRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantOwnershipHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantOwnershipRequirement requirement)
    {
        // 1. إذا كان المستخدم SuperAdmin، امنحه الصلاحية فوراً
        if (context.User.IsInRole("SuperAdmin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 2. جلب الـ TenantId من التوكن
        var userTenantId = context.User.FindFirst("tenantId")?.Value;

        // 3. جلب الـ ID المطلوب من الـ URL (المسار)
        var routeData = _httpContextAccessor.HttpContext?.GetRouteData();
        var requestedId = routeData?.Values["id"]?.ToString();

        // 4. المقارنة
        if (userTenantId != null && userTenantId.Equals(requestedId, StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}