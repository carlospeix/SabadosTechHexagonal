using Persistence;

namespace WebApi;

internal class ConstantTenantProvider : ITenantProvider
{
    public int GetTenantId() => 0;
}