namespace Tests;

[SetUpFixture]
public class Bootstrap : BaseTests
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        if (CreateContext().Database.IsInMemory())
        {
            return;
        }

        CreateContext().Database.Migrate();
    }
}
