namespace Tests;

[SetUpFixture]
public class Bootstrap : BaseTests
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        CreateContext().Database.Migrate();
    }
}
