namespace Persistence;

public class ConstantTenantProvider : ITenantProvider
{
    public int GetTenantId() => 0;
}
