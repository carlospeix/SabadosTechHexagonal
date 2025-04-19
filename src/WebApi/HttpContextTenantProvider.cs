using Persistence;

namespace WebApi;

internal class HttpContextTenantProvider : ITenantProvider
{
    private const string TenantIdHeaderName = "X-TenantId";
    private readonly int[] AllowedTenants = { 1, 8 };

    private readonly IHttpContextAccessor httpContextAccessor;
    //private readonly TenantSettings _tenantSettings;

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor) //, IOptions<TenantSettings> tenantsOptions)
    {
        this.httpContextAccessor = httpContextAccessor;
        //AllowedTenants = tenantsOptions.Value;
    }

    public int GetTenantId()
    {
        var tenantIdHeader = httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeaderName];

        if (tenantIdHeader.HasValue && int.TryParse(tenantIdHeader.Value, out int tenantld) && AllowedTenants.Contains(tenantld))
        {
            return tenantld;
        }

        throw new ApplicationException("Tenant ID is not present");
    }
}
