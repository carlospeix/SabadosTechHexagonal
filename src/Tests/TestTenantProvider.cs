using Persistence;

namespace Tests;

internal class TestTenantProvider(int tenantId) : ITenantProvider
{
    private int tenantId = tenantId;

    internal void SetTenantId(int tenantId) => this.tenantId = tenantId;

    public int GetTenantId() => tenantId;
}