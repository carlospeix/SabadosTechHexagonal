namespace Tests;

public class PersistenceTests
{
    ApplicationContext _context;

    [SetUp]
    public void Setup()
    {
        _context = new ApplicationContext();
        _context.Configurations.RemoveRange(_context.Configurations);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.ChangeTracker.Clear();
        _context.Configurations.RemoveRange(_context.Configurations);
        _context.SaveChanges();
        _context.Dispose();
    }

    [Test]
    public void ConfigurationsIEmpty()
    {
        Assert.That(_context.Configurations.Count(), Is.EqualTo(0));
    }

    [Test]
    public void ShouldFailBecauseOfConfigurationNameDuplicated()
    {
        _context.Configurations.Add(new Configuration("OneName", "one value"));
        _context.SaveChanges();
        Assert.Throws<DbUpdateException>(delegate
        {
            _context.Configurations.Add(new Configuration("OneName", "another value"));
            _context.SaveChanges();
        });
    }
}
