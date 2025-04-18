using Persistence;

namespace Tests;

internal class TestTenantProvider : ITenantProvider
{
    private int tenantId;

    public TestTenantProvider(int tenantId) => this.tenantId = tenantId;

    internal void SetTenantId(int tenantId) => this.tenantId = tenantId;

    public int GetTenantId() => tenantId;
}