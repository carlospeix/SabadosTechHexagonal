namespace Tests;

[SetUpFixture]
public class Bootstrap : BaseTests
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var dataContext = CreateContext();

        if (dataContext.Database.IsInMemory())
        {
            return;
        }

        dataContext.Database.EnsureDeleted();
        dataContext.Database.EnsureCreated();
    }
}
